using System.Linq;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.RandomPrompt;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.RandomPrompt
{
	public class RandomPromptQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly RandomPromptQueryHandler _handler;

		public RandomPromptQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new RandomPromptQueryHandler(DbContext);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(3)]
		[InlineData(10)]
		[InlineData(200)]
		public async Task Handle_ReturnsAnIdThatExistsIntheDatabase_WhenThereArePromptsInDatabase(int amount)
		{
			//arrange
			for (var i = 0; i < amount; i++)
			{
				DbContext.Prompts.Add(new Prompt());
			}

			await DbContext.SaveChangesAsync();
			var query = new RandomPromptQuery();

			//act
			RandomPromptViewModel? actual = await _handler.Handle(query);

			//assert
			Assert.True(DbContext.Prompts.Any(e => e.Id == actual.Id));
		}

		[Fact]
		public async Task Handle_ReturnsNull_WhenThereAreNoPromptsInDatabase()
		{
			//arrange
			var query = new RandomPromptQuery();

			//act
			RandomPromptViewModel? actual = await _handler.Handle(query);

			//assert
			Assert.Null(actual);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(3)]
		[InlineData(10)]
		[InlineData(200)]
		public async Task Handle_ReturnsNull_WhenThereAreOnlyDraftPromptsInDatabase(int amount)
		{
			//arrange
			for (var i = 0; i < amount; i++)
			{
				DbContext.Prompts.Add(new Prompt {IsDraft = true});
			}

			await DbContext.SaveChangesAsync();
			var query = new RandomPromptQuery();

			//act
			RandomPromptViewModel? actual = await _handler.Handle(query);

			//assert
			Assert.Null(actual);
		}
	}
}
