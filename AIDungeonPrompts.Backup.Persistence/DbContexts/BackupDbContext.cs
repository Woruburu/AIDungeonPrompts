using AIDungeonPrompts.Backup.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Backup.Persistence.DbContexts
{
	public class BackupDbContext : DbContext
	{
		public BackupDbContext(DbContextOptions<BackupDbContext> options)
			: base(options)
		{
		}

		public DbSet<BackupPrompt> Prompts { get; set; }
		public DbSet<BackupWorldInfo> WorldInfos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) =>
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackupDbContext).Assembly);
	}
}
