using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetPrompt
{
	public class GetPromptQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly GetPromptQueryHandler _handler;

		public GetPromptQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new GetPromptQueryHandler(DbContext);
		}

		[Theory]
		[InlineData(1, 0)]
		[InlineData(3, 1)]
		[InlineData(10, 4)]
		[InlineData(200, 56)]
		public async Task Handle_ReturnsGivenPrompt(int amount, int index)
		{
			//arrange
			const string expectedTitle = "TestTitle";
			const string expectedContent = "TestContent";
			var prompt = new Prompt();
			for (var i = 0; i < amount; i++)
			{
				if (i == index)
				{
					prompt = new Prompt() { Title = expectedTitle, PromptContent = expectedContent };
					DbContext.Prompts.Add(prompt);
					await DbContext.SaveChangesAsync();
				}
				else
				{
					DbContext.Prompts.Add(new Prompt());
				}
			}
			await DbContext.SaveChangesAsync();
			var query = new GetPromptQuery(prompt.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Equal(prompt.Id, result.Id);
			Assert.Equal(expectedTitle, result.Title);
			Assert.Equal(expectedContent, result.PromptContent);
		}

		[Theory]
		[InlineData(-10)]
		[InlineData(0)]
		[InlineData(64)]
		[InlineData(256)]
		[InlineData(int.MaxValue)]
		public async Task Handle_ReturnsNull_WhenTheDatabaseIsEmpty(int id)
		{
			//arrange
			var query = new GetPromptQuery(id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
		}

		[Theory]
		[InlineData(-10)]
		[InlineData(0)]
		[InlineData(int.MaxValue)]
		public async Task Handle_ReturnsNull_WhenTheNoEntryWithTheGivenIdExists(int id)
		{
			//arrange
			var query = new GetPromptQuery(id);
			DbContext.Prompts.AddRange(new Prompt(), new Prompt(), new Prompt());
			await DbContext.SaveChangesAsync();

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
		}
	}
}
