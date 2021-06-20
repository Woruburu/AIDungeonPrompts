using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Backup.Persistence.DbContexts;
using AIDungeonPrompts.Web.HostedServices.DatabaseBackups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class DatabaseBackupHostedService : IHostedService
	{
		private readonly ILogger<DatabaseBackupHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DatabaseBackupHostedService(
			ILogger<DatabaseBackupHostedService> logger,
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
				_logger.LogInformation($"{nameof(DatabaseBackupHostedService)} Running Job");
				using IServiceScope? services = _serviceScopeFactory.CreateScope();

				using IAIDungeonPromptsDbContext? dbContext =
					services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
				if (dbContext == null)
				{
					_logger.LogWarning($"{nameof(DatabaseBackupHostedService)}: Could not get DbContext from services");
					return;
				}

				using BackupDbContext? backupContext = services.ServiceProvider.GetRequiredService<BackupDbContext>();
				if (backupContext == null)
				{
					_logger.LogWarning(
						$"{nameof(DatabaseBackupHostedService)}: Could not get Backup DbContext from services");
					return;
				}

				await DatabaseBackup.BackupDatabase(dbContext, backupContext, cancellationToken);

				_logger.LogInformation($"{nameof(DatabaseBackupHostedService)} Job Complete");
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(DatabaseBackupHostedService)} Job Failed");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
