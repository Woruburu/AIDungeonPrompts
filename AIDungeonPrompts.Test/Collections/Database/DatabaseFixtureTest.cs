using System;
using AIDungeonPrompts.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AIDungeonPrompts.Test.Collections.Database
{
	[Collection("DatabaseFixture")]
	public abstract class DatabaseFixtureTest : IDisposable
	{
		protected DatabaseFixtureTest(DatabaseFixture fixture)
		{
			DbContext = fixture.DbContext;
		}

		public AIDungeonPromptsDbContext DbContext { get; }

		public void Dispose()
		{
			foreach (var dbEntityEntry in DbContext.ChangeTracker.Entries())
			{
				if (dbEntityEntry.Entity != null)
				{
					dbEntityEntry.State = EntityState.Detached;
				}
			}

			DbContext.AuditPrompts.RemoveRange(DbContext.AuditPrompts);
			DbContext.WorldInfos.RemoveRange(DbContext.WorldInfos);
			DbContext.Reports.RemoveRange(DbContext.Reports);
			DbContext.PromptTags.RemoveRange(DbContext.PromptTags);
			DbContext.Prompts.RemoveRange(DbContext.Prompts);
			DbContext.Tags.RemoveRange(DbContext.Tags);
			DbContext.Users.RemoveRange(DbContext.Users);
			DbContext.SaveChanges();
		}
	}
}
