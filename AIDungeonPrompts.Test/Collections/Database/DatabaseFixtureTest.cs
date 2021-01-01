using System;
using AIDungeonPrompts.Persistence.DbContexts;
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

		internal AIDungeonPromptsDbContext DbContext { get; }

		public void Dispose()
		{
			DbContext.ApplicationLogs.RemoveRange(DbContext.ApplicationLogs);
			DbContext.WorldInfos.RemoveRange(DbContext.WorldInfos);
			DbContext.Reports.RemoveRange(DbContext.Reports);
			DbContext.Prompts.RemoveRange(DbContext.Prompts);
			DbContext.Tags.RemoveRange(DbContext.Tags);
			DbContext.PromptTags.RemoveRange(DbContext.PromptTags);
			DbContext.AuditPrompts.RemoveRange(DbContext.AuditPrompts);
			DbContext.Users.RemoveRange(DbContext.Users);
			DbContext.SaveChanges();
		}
	}
}
