using System;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.CreatePrompt
{
	public class CreatePromptCommandHandlerTest : DatabaseFixtureTest
	{
		private readonly CreatePromptCommandHandler _handler;

		public CreatePromptCommandHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new CreatePromptCommandHandler(DbContext);
		}

		[Fact]
		public async Task Handle_AddsANewPromptToTheDatabase_AndReturnsThePromptId()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand { OwnerId = user.Id };

			//act
			var actual = await _handler.Handle(command);

			//assert
			Assert.Single(DbContext.Prompts);
			Assert.NotNull(DbContext.Prompts.Find(actual));
		}

		[Fact]
		public async Task Handle_DoesNotSetPromptToDraft_WhenSaveDraftIsFalse()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand { OwnerId = user.Id, SaveDraft = false };

			//act
			var actual = await _handler.Handle(command);

			//assert
			var prompt = DbContext.Prompts.Find(actual);
			Assert.False(prompt.IsDraft);
		}

		[Fact]
		public async Task Handle_SetsPromptToDraft_WhenSaveDraftIsTrue()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand { OwnerId = user.Id, SaveDraft = true };

			//act
			var actual = await _handler.Handle(command);

			//assert
			var prompt = DbContext.Prompts.Find(actual);
			Assert.True(prompt.IsDraft);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-10)]
		[InlineData(10)]
		[InlineData(256)]
		public async Task Handle_Throws_WhenThereIsNoValidOwner(int ownerId)
		{
			//arrange
			var command = new CreatePromptCommand() { OwnerId = ownerId };

			//act + assert
			await Assert.ThrowsAnyAsync<Exception>(async () => await _handler.Handle(command));
		}
	}
}
