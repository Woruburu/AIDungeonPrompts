using System;
using AIDungeonPrompts.Persistence.DbContexts;
using AIDungeonPrompts.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AIDungeonPrompts.Test.Collections.Database
{
	public class DatabaseFixture : IDisposable
	{
		private const string DatabaseConnectionName = "AIDungeonPrompt";

		public DatabaseFixture()
		{
			DbContextOptions = new DbContextOptionsBuilder<AIDungeonPromptsDbContext>()
					.UseNpgsql(ConfigHelper.GetConfiguration().GetConnectionString(DatabaseConnectionName))
					.Options;
			using var dbContext = new AIDungeonPromptsDbContext(DbContextOptions);
			dbContext.Database.EnsureCreated();
		}

		public DbContextOptions<AIDungeonPromptsDbContext> DbContextOptions { get; }

		public void Dispose()
		{
			using var dbContext = new AIDungeonPromptsDbContext(DbContextOptions);
			dbContext.Database.EnsureDeleted();
		}
	}
}
