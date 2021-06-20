using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Web.HostedServices.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class ReportCleanerCronJob : CronJobHostedService
	{
		private readonly ILogger<ReportCleanerCronJob> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public ReportCleanerCronJob(
			ILogger<ReportCleanerCronJob> logger,
			IServiceScopeFactory serviceScopeFactory
		) : base("0 0 * * *", TimeZoneInfo.Local, logger)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public override async Task DoWork(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ReportCleanerCronJob)} Running Job");
			using IServiceScope? services = _serviceScopeFactory.CreateScope();
			using IAIDungeonPromptsDbContext? dbContext =
				services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
			if (dbContext == null)
			{
				_logger.LogWarning($"{nameof(ReportCleanerCronJob)}: Could not get DbContext from services");
				return;
			}

			List<Report>? reportsToRemove = await dbContext
				.Reports
				.Where(e => e.Cleared)
				.ToListAsync(cancellationToken);
			_logger.LogInformation($"Removing {reportsToRemove.Count} reports.");
			dbContext.Reports.RemoveRange(reportsToRemove);
			await dbContext.SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"{nameof(ReportCleanerCronJob)} Job Complete");
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ReportCleanerCronJob)} Starting");
			await base.StartAsync(cancellationToken);
			_logger.LogInformation($"{nameof(ReportCleanerCronJob)} Started");
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ReportCleanerCronJob)} Stopping");
			await base.StopAsync(cancellationToken);
			_logger.LogInformation($"{nameof(ReportCleanerCronJob)} Stopped");
		}
	}
}
