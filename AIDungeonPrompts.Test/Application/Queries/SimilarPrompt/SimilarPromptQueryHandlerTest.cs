using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.SimilarPrompt
{
	public class SimilarPromptQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly SimilarPromptQueryHandler _handler;

		public SimilarPromptQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new SimilarPromptQueryHandler(DbContext);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(67)]
		[InlineData(256)]
		public async Task Handle_ReturnsExpectedMatches_WhenThereAreTitleMatches_AndOnePromptMatchesId(int expectedMatches)
		{
			//arrange
			var prompt = new Prompt
			{
				Title = "TestTitle",
				PromptContent = "TestContent"
			};
			DbContext.Prompts.Add(prompt);
			for (var i = 0; i < expectedMatches; i++)
			{
				DbContext.Prompts.Add(new Prompt
				{
					Title = "TestTitle",
					PromptContent = "TestContent"
				});
			}
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("TestTitle", prompt.Id);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Matched);
			Assert.Equal(expectedMatches, actual.SimilarPrompts.Count);
		}

		[Theory]
		[InlineData(3)]
		[InlineData(10)]
		[InlineData(38)]
		[InlineData(120)]
		public async Task Handle_ReturnsMatches_WhenManyTitlesMatch(int expectedAmount)
		{
			//arrange
			for (var i = 0; i < expectedAmount; i++)
			{
				DbContext.Prompts.Add(new Prompt
				{
					Title = "TestTitle",
					PromptContent = "TestContent"
				});
			}
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("TestTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Matched);
			Assert.Equal(expectedAmount, actual.SimilarPrompts.Count);
		}

		[Theory]
		[InlineData(3, 3)]
		[InlineData(10, 20)]
		[InlineData(38, 60)]
		[InlineData(120, 45)]
		public async Task Handle_ReturnsMatchesThatDontIncludeDrafts_WhenManyTitlesMatch_ButSomeAreDrafts(int expectedAmount, int drafts)
		{
			//arrange
			for (var i = 0; i < expectedAmount; i++)
			{
				DbContext.Prompts.Add(new Prompt
				{
					Title = "TestTitle",
					PromptContent = "TestContent"
				});
			}
			for (var i = 0; i < drafts; i++)
			{
				DbContext.Prompts.Add(new Prompt
				{
					Title = "TestTitle",
					PromptContent = "TestContent",
					IsDraft = true
				});
			}
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("TestTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Matched);
			Assert.Equal(expectedAmount, actual.SimilarPrompts.Count);
		}

		[Fact]
		public async Task Handle_ReturnsNoMatches_WhenDatabaseIsEmpty()
		{
			//arrange
			var query = new SimilarPromptQuery("TestTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.False(actual.Matched);
			Assert.Empty(actual.SimilarPrompts);
		}

		[Fact]
		public async Task Handle_ReturnsNoMatches_WhenOneTitleMatches_ButIsDraft()
		{
			//arrange
			DbContext.Prompts.Add(new Prompt
			{
				Title = "TestTitle",
				PromptContent = "TestContent",
				IsDraft = true
			});
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("TestTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.False(actual.Matched);
			Assert.Empty(actual.SimilarPrompts);
		}

		[Fact]
		public async Task Handle_ReturnsNoMatches_WhenThereAreNoSimilarPrompts()
		{
			//arrange
			DbContext.Prompts.Add(new Prompt
			{
				Title = "TestTitle",
				PromptContent = "TestContent"
			});
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("NewTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.False(actual.Matched);
			Assert.Empty(actual.SimilarPrompts);
		}

		[Fact]
		public async Task Handle_ReturnsOneMatch_WhenOneTitleMatches()
		{
			//arrange
			DbContext.Prompts.Add(new Prompt
			{
				Title = "TestTitle",
				PromptContent = "TestContent"
			});
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("TestTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Matched);
			Assert.Single(actual.SimilarPrompts);
		}

		[Fact]
		public async Task Handle_ReturnsOneMatch_WhenTwoTitlesMatch_ButOneIsDraft()
		{
			//arrange
			DbContext.Prompts.Add(new Prompt
			{
				Title = "TestTitle",
				PromptContent = "TestContent",
				IsDraft = true
			});
			DbContext.Prompts.Add(new Prompt
			{
				Title = "TestTitle",
				PromptContent = "TestContent",
				IsDraft = false
			});
			await DbContext.SaveChangesAsync();
			var query = new SimilarPromptQuery("TestTitle");

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Matched);
			Assert.Single(actual.SimilarPrompts);
		}
	}
}
