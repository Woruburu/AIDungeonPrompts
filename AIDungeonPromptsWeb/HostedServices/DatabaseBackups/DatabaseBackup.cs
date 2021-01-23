using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Backup.Persistence.DbContexts;
using AIDungeonPrompts.Backup.Persistence.Entities;
using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Web.HostedServices.DatabaseBackups
{
	public static class DatabaseBackup
	{
		public async static Task BackupDatabase(IAIDungeonPromptsDbContext dbContext, BackupDbContext backupContext, CancellationToken cancellationToken = default)
		{
			await CleanBackup(backupContext, cancellationToken);

			var nonDrafts = await dbContext.NonDraftPrompts.Select(e => e.Id).ToListAsync(cancellationToken);

			var page = 0;
			const int pageSize = 100;
			var totalCount = await dbContext.Prompts.CountAsync(cancellationToken);

			while (page * pageSize < totalCount)
			{
				var allPrompts = await dbContext
					.Prompts
					.Include(e => e.PromptTags)
					.ThenInclude(e => e.Tag)
					.Include(e => e.WorldInfos)
					.OrderBy(e => e.Id)
					.Skip(page * pageSize)
					.Take(pageSize)
					.AsNoTracking()
					.ToListAsync(cancellationToken);
				var backups = allPrompts.Where(e => nonDrafts.Contains(e.Id)).Select(prompt => CreateBackupPrompt(prompt)).ToList();
				backupContext.Prompts.AddRange(backups);
				await backupContext.SaveChangesAsync(cancellationToken);
				page++;
			}
		}

		private static async Task CleanBackup(BackupDbContext context, CancellationToken cancellationToken)
		{
			var promptTableName = context.Model.FindEntityType(typeof(BackupPrompt)).GetTableName();
			var worldInfoTableName = context.Model.FindEntityType(typeof(BackupWorldInfo)).GetTableName();

			await using var command = context.Database.GetDbConnection().CreateCommand();
			command.CommandText = $"PRAGMA journal_mode = NONE;DELETE FROM {worldInfoTableName};DELETE FROM sqlite_sequence WHERE name='{worldInfoTableName}';DELETE FROM {promptTableName};DELETE FROM sqlite_sequence WHERE name='{promptTableName}';VACUUM";
			command.CommandType = CommandType.Text;
			await context.Database.OpenConnectionAsync(cancellationToken);
			await command.ExecuteNonQueryAsync(cancellationToken);
			await context.Database.CloseConnectionAsync();
		}

		private static BackupPrompt CreateBackupPrompt(Prompt prompt)
		{
			return new BackupPrompt
			{
				AuthorsNote = prompt.AuthorsNote,
				DateCreated = prompt.DateCreated,
				DateEdited = prompt.DateEdited,
				Description = prompt.Description,
				CorrelationId = prompt.Id,
				Memory = prompt.Memory,
				Nsfw = prompt.Nsfw,
				ParentId = prompt.ParentId,
				PromptContent = prompt.PromptContent,
				Quests = prompt.Quests,
				Title = prompt.Title,
				PublishDate = prompt.PublishDate,
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
			};
		}
	}
}
