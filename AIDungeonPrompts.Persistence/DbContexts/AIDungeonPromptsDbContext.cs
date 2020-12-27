using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Entities.Abstract;
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

		public DbSet<ApplicationLog> ApplicationLogs { get; set; }
		public DbSet<AuditPrompt> AuditPrompts { get; set; }
		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
		public DbSet<Prompt> Prompts { get; set; }
		public DbSet<PromptTag> PromptTags { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<WorldInfo> WorldInfos { get; set; }

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			var changedEntities = ChangeTracker.Entries<BaseDomainEntity>()
				.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

			foreach (var entity in changedEntities)
			{
				switch (entity.State)
				{
					case EntityState.Added:
						if (entity.Entity.DateCreated == default)
						{
							entity.Entity.DateCreated = DateTime.UtcNow;
						}
						break;

					case EntityState.Modified:
						if (entity.Entity.DateEdited == default)
						{
							entity.Entity.DateEdited = DateTime.UtcNow;
						}
						break;
				}
			}

			return await base.SaveChangesAsync(cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) =>
					modelBuilder.ApplyConfigurationsFromAssembly(typeof(AIDungeonPromptsDbContext).Assembly);
	}
}
