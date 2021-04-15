using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AIDungeonPrompts.Application.Abstractions.DbContexts
{
	public interface IAIDungeonPromptsDbContext : IDisposable
	{
		DbSet<ApplicationLog> ApplicationLogs { get; set; }
		DbSet<AuditPrompt> AuditPrompts { get; set; }
		DatabaseFacade Database { get; }
		DbSet<NonDraftPrompt> NonDraftPrompts { get; set; }
		DbSet<Prompt> Prompts { get; set; }
		DbSet<PromptTag> PromptTags { get; set; }
		DbSet<Report> Reports { get; set; }
		DbSet<Tag> Tags { get; set; }
		DbSet<User> Users { get; set; }
		DbSet<WorldInfo> WorldInfos { get; set; }
		DbSet<ServerFlag> ServerFlags { get; set; }

		EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

		EntityEntry Entry(object entity);

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
