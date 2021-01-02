using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class NewlineFixerHostedService : IHostedService
	{
		private readonly ILogger<NewlineFixerHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public NewlineFixerHostedService(
			ILogger<NewlineFixerHostedService> logger,
			IServiceScopeFactory serviceScopeFactory
		)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(NewlineFixerHostedService)} Starting");
			using var services = _serviceScopeFactory.CreateScope();
			var dbContext = services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
			if (dbContext == null)
			{
				_logger.LogWarning($"{nameof(NewlineFixerHostedService)}: Could not get DbContext from services");
				return;
			}

			var allPrompts = await dbContext.Prompts.Include(e => e.WorldInfos).ToListAsync(cancellationToken);
			_logger.LogInformation($"Updating {allPrompts.Count} prompts from \\r\\n to \\n");
			foreach (var prompt in allPrompts)
			{
				prompt.AuthorsNote = prompt.AuthorsNote?.Replace("\r\n", "\n");
				prompt.Memory = prompt.Memory?.Replace("\r\n", "\n");
				prompt.PromptContent = prompt.PromptContent.Replace("\r\n", "\n");
				prompt.Quests = prompt.Quests?.Replace("\r\n", "\n");
				prompt.Title = prompt.Title.Replace("\r\n", "\n");
				prompt.Description = prompt.Description?.Replace("\r\n", "\n");
				foreach (var wi in prompt.WorldInfos)
				{
					wi.Entry = wi.Entry.Replace("\r\n", "\n");
					wi.Keys = wi.Keys.Replace("\r\n", "\n");
				}
			}
			dbContext.Prompts.UpdateRange(allPrompts);
			await dbContext.SaveChangesAsync(cancellationToken);
			_logger.LogInformation($"{nameof(NewlineFixerHostedService)} Finished");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
