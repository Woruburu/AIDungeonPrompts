using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetScript;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetScript
{
	public class GetScriptQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly GetScriptQueryHandler _handler;

		public GetScriptQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new GetScriptQueryHandler(DbContext);
		}

		[Fact]
		public async Task Handle_ReturnsNull_WhenRelatedPromptHasNoScriptBytes()
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var prompt = new Prompt()
			{
				Owner = owner,
				ScriptZip = null
			};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var query = new GetScriptQuery(prompt.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
		}

		[Theory]
		[InlineData(new byte[] { 0x50, 0x4b, 0x03, 0x04 })]
		[InlineData(new byte[] { 0x50, 0x4b, 0x05, 0x06 })]
		[InlineData(new byte[] { 0x50, 0x4b, 0x07, 0x08 })]
		public async Task Handle_ReturnsScriptBytes_WhenRelatedPromptHasScriptBytes(byte[] expectedBytes)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var prompt = new Prompt()
			{
				Owner = owner,
				ScriptZip = expectedBytes
			};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var query = new GetScriptQuery(prompt.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedBytes, result);
		}
	}
}
