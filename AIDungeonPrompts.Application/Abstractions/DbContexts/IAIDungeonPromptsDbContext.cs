using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Abstractions.DbContexts
{
	public interface IAIDungeonPromptsDbContext : IDisposable
	{
		DbSet<ApplicationLog> ApplicationLogs { get; set; }
		DbSet<AuditPrompt> AuditPrompts { get; set; }
		DbSet<Prompt> Prompts { get; set; }
		DbSet<PromptTag> PromptTags { get; set; }
		DbSet<Report> Reports { get; set; }
		DbSet<Tag> Tags { get; set; }
		DbSet<User> Users { get; set; }
		DbSet<WorldInfo> WorldInfos { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
