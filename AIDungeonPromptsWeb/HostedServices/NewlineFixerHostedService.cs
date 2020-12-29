using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class NewlineFixerHostedService : IHostedService, IDisposable
	{
		private readonly ILogger<NewlineFixerHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private bool _disposedValue;

		public NewlineFixerHostedService(
			ILogger<NewlineFixerHostedService> logger,
			IServiceScopeFactory serviceScopeFactory
		)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Starting");
			using var services = _serviceScopeFactory.CreateScope();
			var dbContext = services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
			if (dbContext == null)
			{
				_logger.LogWarning($"{nameof(ApplicationLogCleanerHostedService)}: Could not get DbContext from services");
				return;
			}

			var allPrompts = await dbContext.Prompts.Include(e => e.WorldInfos).ToListAsync();
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
			_logger.LogInformation($"{nameof(ApplicationLogCleanerHostedService)} Finished");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~NewlineFixerHostedService()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }
	}
}
