using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Backup.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class DatabaseMigrationHostedService : IHostedService
	{
		private readonly ILogger<DatabaseMigrationHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DatabaseMigrationHostedService(
			ILogger<DatabaseMigrationHostedService> logger,
			IServiceScopeFactory serviceScopeFactory
		)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation($"{nameof(DatabaseMigrationHostedService)} Running Job");
				using IServiceScope? services = _serviceScopeFactory.CreateScope();

				using IAIDungeonPromptsDbContext? dbContext =
					services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
				if (dbContext == null)
				{
					_logger.LogWarning(
						$"{nameof(DatabaseMigrationHostedService)}: Could not get DbContext from services");
					return;
				}

				using BackupDbContext? backupContext = services.ServiceProvider.GetRequiredService<BackupDbContext>();
				if (backupContext == null)
				{
					_logger.LogWarning(
						$"{nameof(DatabaseMigrationHostedService)}: Could not get Backup DbContext from services");
					return;
				}

				await dbContext.Database.MigrateAsync(cancellationToken);

				await using DbCommand? command = backupContext.Database.GetDbConnection().CreateCommand();
				command.CommandText = "PRAGMA journal_mode = NONE;";
				command.CommandType = CommandType.Text;
				await backupContext.Database.OpenConnectionAsync(cancellationToken);
				await command.ExecuteNonQueryAsync(cancellationToken);
				await backupContext.Database.CloseConnectionAsync();
				await backupContext.Database.EnsureCreatedAsync(cancellationToken);

				_logger.LogInformation($"{nameof(DatabaseMigrationHostedService)} Job Complete");
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(DatabaseMigrationHostedService)} Job Failed");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
