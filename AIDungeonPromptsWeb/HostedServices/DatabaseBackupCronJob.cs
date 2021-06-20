using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Backup.Persistence.DbContexts;
using AIDungeonPrompts.Web.HostedServices.Abstracts;
using AIDungeonPrompts.Web.HostedServices.DatabaseBackups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class DatabaseBackupCronJob : CronJobHostedService
	{
		private readonly ILogger<ApplicationLogCleanerCronJob> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DatabaseBackupCronJob(
			ILogger<ApplicationLogCleanerCronJob> logger,
			IServiceScopeFactory serviceScopeFactory
		) : base("0 * * * *", TimeZoneInfo.Local, logger)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public override async Task DoWork(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(DatabaseBackupCronJob)} Running Job");
			using IServiceScope? services = _serviceScopeFactory.CreateScope();

			using IAIDungeonPromptsDbContext? dbContext =
				services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
			if (dbContext == null)
			{
				_logger.LogWarning($"{nameof(DatabaseBackupCronJob)}: Could not get DbContext from services");
				return;
			}

			using BackupDbContext? backupContext = services.ServiceProvider.GetRequiredService<BackupDbContext>();
			if (backupContext == null)
			{
				_logger.LogWarning($"{nameof(DatabaseBackupCronJob)}: Could not get Backup DbContext from services");
				return;
			}

			await DatabaseBackup.BackupDatabase(dbContext, backupContext, cancellationToken);

			_logger.LogInformation($"{nameof(DatabaseBackupCronJob)} Job Complete");
		}
	}
}
