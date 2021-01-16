using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Backup.Persistence.DbContexts;
using AIDungeonPrompts.Backup.Persistence.Entities;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Web.HostedServices.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.HostedServices
{
	public class DatabaseBackupHostedService : CronJobHostedService
	{
		private readonly ILogger<ApplicationLogCleanerHostedService> _logger;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DatabaseBackupHostedService(
			ILogger<ApplicationLogCleanerHostedService> logger,
			IServiceScopeFactory serviceScopeFactory
		) : base("30 18 * * *", TimeZoneInfo.Local, logger)
		{
			_logger = logger;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public override async Task DoWork(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(DatabaseBackupHostedService)} Running Job");
			using var services = _serviceScopeFactory.CreateScope();

			using var dbContext = services.ServiceProvider.GetRequiredService<IAIDungeonPromptsDbContext>();
			if (dbContext == null)
			{
				_logger.LogWarning($"{nameof(DatabaseBackupHostedService)}: Could not get DbContext from services");
				return;
			}

			using var backupContext = services.ServiceProvider.GetRequiredService<BackupDbContext>();
			if (backupContext == null)
			{
				_logger.LogWarning($"{nameof(DatabaseBackupHostedService)}: Could not get Backup DbContext from services");
				return;
			}

			await CleanBackup(backupContext);

			var page = 0;
			var pageSize = 100;
			var totalCount = await dbContext.Prompts.CountAsync();

			while (page * pageSize < totalCount)
			{
				var allPrompts = await dbContext
					.Prompts
					.Include(e => e.PromptTags)
					.ThenInclude(e => e.Tag)
					.Include(e => e.WorldInfos)
					.Skip(page * pageSize)
					.Take(pageSize)
					.AsNoTracking()
					.ToListAsync(cancellationToken);
				foreach (var prompt in allPrompts)
				{
					if (await IsDraft(dbContext, prompt.Id))
					{
						continue;
					}
					AddBackupPrompt(backupContext, prompt);
				}
				await backupContext.SaveChangesAsync();
				page++;
			}

			var backupDbCount = await backupContext.Prompts.CountAsync();
			Console.WriteLine($"Main DB Count: {totalCount}");
			Console.WriteLine($"Backup db count: {backupDbCount}");
		}

		private void AddBackupPrompt(BackupDbContext context, Prompt prompt)
		{
			context.Prompts.Add(new BackupPrompt
			{
				AuthorsNote = prompt.AuthorsNote,
				DateCreated = prompt.DateCreated,
				DateEdited = prompt.DateEdited,
				Description = prompt.Description,
				CorrelationId = prompt.Id,
				IsDraft = false,
				Memory = prompt.Memory,
				Nsfw = prompt.Nsfw,
				ParentId = prompt.ParentId,
				PromptContent = prompt.PromptContent,
				Quests = prompt.Quests,
				Title = prompt.Title,
				Tags = string.Join(", ", prompt.PromptTags.Select(e => e.Tag!.Name)),
				WorldInfos = prompt.WorldInfos.Select(worldInfo => new BackupWorldInfo
				{
					DateCreated = worldInfo.DateCreated,
					DateEdited = worldInfo.DateEdited,
					Entry = worldInfo.Entry,
					CorrelationId = worldInfo.Id,
					Keys = worldInfo.Keys,
					PromptId = worldInfo.PromptId
				}).ToList()
			});
		}

		private async Task CleanBackup(BackupDbContext context)
		{
			await context.Database.EnsureDeletedAsync();
			context.Database.Migrate();
			using (var command = context.Database.GetDbConnection().CreateCommand())
			{
				command.CommandText = "DELETE FROM Prompts;VACUUM;";
				command.CommandType = CommandType.Text;

				context.Database.OpenConnection();

				await command.ExecuteNonQueryAsync();
			}
		}

		private async Task<bool> IsDraft(IAIDungeonPromptsDbContext context, int id)
		{
			if (context is not DbContext dbContext)
			{
				throw new Exception($"{nameof(context)} was not a DbContext");
			}

			var findParent = await context.Prompts.Include(e => e.Parent).FirstOrDefaultAsync(e => e.Id == id);
			while (findParent!.ParentId != null)
			{
				await dbContext.Entry(findParent).Reference(e => e.Parent).LoadAsync();
				findParent = findParent.Parent;
			}
			return findParent.IsDraft;
		}
	}
}
