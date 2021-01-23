using System;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Moq;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.CreatePrompt
{
	public class CreatePromptCommandHandlerTest : DatabaseFixtureTest
	{
		private readonly CreatePromptCommandHandler _handler;
		private readonly Mock<ICurrentUserService> _mockUserService;

		public CreatePromptCommandHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_mockUserService = new Mock<ICurrentUserService>();
			_handler = new CreatePromptCommandHandler(DbContext, _mockUserService.Object);
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
		public async Task Handle_CreatesAPromptWithANullPublishDate_WhenItIsDraft()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand
			{
				OwnerId = user.Id,
				SaveDraft = true
			};

			//act
			var actual = await _handler.Handle(command);

			//assert
			var actualPrompt = await DbContext.Prompts.FindAsync(actual);
			Assert.Null(actualPrompt.PublishDate);
		}

		[Fact]
		public async Task Handle_CreatesAPromptWithAPublishDate_WhenItIsNotADraft()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand
			{
				OwnerId = user.Id,
				SaveDraft = false
			};

			//act
			var actual = await _handler.Handle(command);

			//assert
			var actualPrompt = await DbContext.Prompts.FindAsync(actual);
			Assert.NotNull(actualPrompt.PublishDate);
		}

		[Fact]
		public async Task Handle_CreatesAPromptWithSCriptZipSetToExpectedBytes()
		{
			//arrange
			var expectedBytes = new byte[] { };
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand { OwnerId = user.Id, ScriptZip = expectedBytes };

			//act
			var actualId = await _handler.Handle(command);

			//assert
			var actualPrompt = await DbContext.Prompts.FindAsync(actualId);
			Assert.Equal(expectedBytes, actualPrompt.ScriptZip);
		}

		[Fact]
		public async Task Handle_CreatesAPromptWithScriptZipSetToNull()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand { OwnerId = user.Id, ScriptZip = null };

			//act
			var actualId = await _handler.Handle(command);

			//assert
			var actualPrompt = await DbContext.Prompts.FindAsync(actualId);
			Assert.Null(actualPrompt.ScriptZip);
		}

		[Fact]
		public async Task Handle_CreatesAPromptWithTheParentId_WhenParentIdIsGiven_AndTheCurrentUserIsTheSameId()
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			DbContext.Users.Add(owner);
			await DbContext.SaveChangesAsync();
			var parent = new Prompt
			{
				OwnerId = owner.Id
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new CreatePromptCommand { ParentId = parent.Id, OwnerId = owner.Id };

			//act
			var actual = await _handler.Handle(command);

			//assert
			var actualPrompt = await DbContext.Prompts.FindAsync(actual);
			Assert.NotNull(actualPrompt);
			Assert.Equal(parent.Id, actualPrompt.ParentId);
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

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task Handle_SetsDraftToFalse_WhenThereIsAParentId(bool intitialStatus)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			DbContext.Users.Add(owner);

			var parent = new Prompt { Owner = owner };
			DbContext.Prompts.Add(parent);

			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new CreatePromptCommand { OwnerId = user.Id, SaveDraft = intitialStatus, ParentId = parent.Id };

			//act
			var actual = await _handler.Handle(command);

			//assert
			var actualPrompt = DbContext.Prompts.Find(actual);
			Assert.False(actualPrompt.IsDraft);
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

		[Fact]
		public async Task Handle_ThrowsCreatePromptUnauthorizedParentException_WhenParentIdIsGiven_AndTheCurrentUserIsNotTheSameId()
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var ownerTwo = new User { Username = "TestUserTwo" };
			DbContext.Users.Add(owner);
			DbContext.Users.Add(ownerTwo);
			await DbContext.SaveChangesAsync();
			var parent = new Prompt
			{
				OwnerId = owner.Id
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel { Id = ownerTwo.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new CreatePromptCommand { ParentId = parent.Id };

			//act + assert
			await Assert.ThrowsAsync<CreatePromptUnauthorizedParentException>(async () => await _handler.Handle(command));
		}

		[Fact]
		public async Task Handle_ThrowsCreatePromptUnauthorizedParentException_WhenParentIdIsGiven_AndWhenThereIsNoCurrentUser()
		{
			//arrange
			var user = new User { Username = "TestUser" };
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var parent = new Prompt
			{
				OwnerId = user.Id
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var command = new CreatePromptCommand { ParentId = parent.Id };

			//act + assert
			await Assert.ThrowsAsync<CreatePromptUnauthorizedParentException>(async () => await _handler.Handle(command));
		}
	}
}
