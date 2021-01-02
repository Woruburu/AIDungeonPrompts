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
			DbContext = new AIDungeonPromptsDbContext(fixture.DbContextOptions);
			DbContextOptions = fixture.DbContextOptions;
		}

		public AIDungeonPromptsDbContext DbContext { get; }
		public DbContextOptions<AIDungeonPromptsDbContext> DbContextOptions { get; }

		public void Dispose()
		{
			var dbContext = new AIDungeonPromptsDbContext(DbContextOptions);
			dbContext.ApplicationLogs.RemoveRange(DbContext.ApplicationLogs);
			dbContext.WorldInfos.RemoveRange(DbContext.WorldInfos);
			dbContext.Reports.RemoveRange(DbContext.Reports);
			dbContext.PromptTags.RemoveRange(DbContext.PromptTags);
			dbContext.Prompts.RemoveRange(DbContext.Prompts);
			dbContext.Tags.RemoveRange(DbContext.Tags);
			dbContext.AuditPrompts.RemoveRange(DbContext.AuditPrompts);
			dbContext.Users.RemoveRange(DbContext.Users);
			dbContext.SaveChanges();
			DbContext.Dispose();
		}
	}
}
