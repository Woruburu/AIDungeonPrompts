using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Backup.Persistence.DbContexts;
using AIDungeonPrompts.Web.HostedServices.DatabaseBackups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class DatabaseMigrationAndBackupHostedService : IHostedService
	{
		private readonly ILogger<DatabaseMigrationAndBackupHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DatabaseMigrationAndBackupHostedService(
			ILogger<DatabaseMigrationAndBackupHostedService> logger,
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
				_logger.LogInformation($"{nameof(DatabaseMigrationAndBackupHostedService)} Running Job");
				using var services = _serviceScopeFactory.CreateScope();

				using var dbContext = services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
				if (dbContext == null)
				{
					_logger.LogWarning($"{nameof(DatabaseMigrationAndBackupHostedService)}: Could not get DbContext from services");
					return;
				}

				using var backupContext = services.ServiceProvider.GetRequiredService<BackupDbContext>();
				if (backupContext == null)
				{
					_logger.LogWarning($"{nameof(DatabaseMigrationAndBackupHostedService)}: Could not get Backup DbContext from services");
					return;
				}

				await dbContext.Database.MigrateAsync(cancellationToken);
				await backupContext.Database.MigrateAsync(cancellationToken);
				await DatabaseBackup.BackupDatabase(dbContext, backupContext, cancellationToken);

				_logger.LogInformation($"{nameof(DatabaseMigrationAndBackupHostedService)} Job Complete");
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(DatabaseMigrationAndBackupHostedService)} Job Failed");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
