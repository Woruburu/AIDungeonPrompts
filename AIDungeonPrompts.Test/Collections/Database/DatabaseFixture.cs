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
			DbContext = new AIDungeonPromptsDbContext(
				new DbContextOptionsBuilder<AIDungeonPromptsDbContext>()
					.UseNpgsql(ConfigHelper.GetConfiguration().GetConnectionString(DatabaseConnectionName))
					.Options
			);
			DbContext.Database.EnsureCreated();
		}

		internal AIDungeonPromptsDbContext DbContext { get; }

		public void Dispose()
		{
			DbContext.Database.EnsureDeleted();
			DbContext.Dispose();
		}
	}
}
