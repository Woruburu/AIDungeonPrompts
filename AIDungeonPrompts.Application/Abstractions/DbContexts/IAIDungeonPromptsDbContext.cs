using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Abstractions.DbContexts
{
	public interface IAIDungeonPromptsDbContext
	{
		public DbSet<Prompt> Prompts { get; set; }
		public DbSet<PromptTag> PromptTags { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<WorldInfo> WorldInfos { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
