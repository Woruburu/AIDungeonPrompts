using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Web.HostedServices.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class ApplicationLogCleanerHostedService : CronJobHostedService
	{
		private readonly ILogger<ApplicationLogCleanerHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public ApplicationLogCleanerHostedService(
			ILogger<ApplicationLogCleanerHostedService> logger,
			IServiceScopeFactory serviceScopeFactory
		) : base("0 0 * * *", TimeZoneInfo.Local)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public override async Task DoWork(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Running Job");
			using var services = _serviceScopeFactory.CreateScope();
			var dbContext = services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
			if (dbContext == null)
			{
				_logger.LogWarning($"{nameof(ApplicationLogCleanerHostedService)}: Could not get DbContext from services");
				return;
			}

			var logsToRemove = await dbContext
				.ApplicationLogs
				.Where(e => e.TimeStamp < DateTime.UtcNow.AddDays(-7))
				.ToListAsync();
			_logger.LogInformation($"Removing {logsToRemove.Count} logs.");
			dbContext.ApplicationLogs.RemoveRange(logsToRemove);
			await dbContext.SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Job Complete");
			return;
		}

		public async override Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Starting");
			await base.StartAsync(cancellationToken);
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Started");
		}

		public async override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Stopping");
			await base.StopAsync(cancellationToken);
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Stopped");
		}
	}
}
