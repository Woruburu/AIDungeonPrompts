using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using Audit.EntityFramework;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Persistence.DbContexts
{
	public class AIDungeonPromptsDbContext : AuditDbContext, IDataProtectionKeyContext, IAIDungeonPromptsDbContext
	{
		public AIDungeonPromptsDbContext(DbContextOptions<AIDungeonPromptsDbContext> options)
			: base(options)
		{
		}

		public DbSet<AuditPrompt> AuditPrompts { get; set; }
		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
		public DbSet<Prompt> Prompts { get; set; }
		public DbSet<PromptTag> PromptTags { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<WorldInfo> WorldInfos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) =>
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AIDungeonPromptsDbContext).Assembly);
	}
}
