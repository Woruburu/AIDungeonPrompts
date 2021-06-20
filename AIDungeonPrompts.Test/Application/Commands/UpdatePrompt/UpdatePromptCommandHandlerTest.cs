using System;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using AIDungeonPrompts.Test.Collections.Database;
using Moq;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.UpdatePrompt
{
	public class UpdatePromptCommandHandlerTest : DatabaseFixtureTest
	{
		private readonly UpdatePromptCommandHandler _handler;
		private readonly Mock<ICurrentUserService> _mockUserService;

		public UpdatePromptCommandHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_mockUserService = new Mock<ICurrentUserService>();
			_handler = new UpdatePromptCommandHandler(DbContext, _mockUserService.Object);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task Handle_DoesNotChangeDraftStatus_WhenUserIsNotOwner(bool expectedDraftStatus)
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {IsDraft = expectedDraftStatus, Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = int.MaxValue, Role = RoleEnum.FieldEdit};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, SaveDraft = !expectedDraftStatus};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.Equal(expectedDraftStatus, actual.IsDraft);
		}

		[Theory]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public async Task Handle_DoesNotChangePublishDate_WhenItHasValue(bool initialDraftStatus,
			bool updatedDraftStatus)
		{
			//arrange
			DateTime expectedDate = DateTime.UtcNow;
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {IsDraft = initialDraftStatus, Owner = owner, PublishDate = expectedDate};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id, Role = RoleEnum.FieldEdit};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, SaveDraft = updatedDraftStatus};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.Equal(expectedDate, actual.PublishDate);
		}

		[Fact]
		public async Task Handle_LeavesPublishNull_WhenSaveDraftIsTrue_AndPromptHasNoPublishDate_AndWasDraft()
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {IsDraft = true, Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id, Role = RoleEnum.FieldEdit};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, SaveDraft = true};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.Null(actual.PublishDate);
		}

		[Fact]
		public async Task Handle_SetsAPromptWithScriptZipSetToExpectedBytes()
		{
			//arrange
			var expectedBytes = new byte[] { };
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, OwnerId = user.Id, ScriptZip = expectedBytes};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actualPrompt = await DbContext.Prompts.FindAsync(prompt.Id);
			Assert.Equal(expectedBytes, actualPrompt.ScriptZip);
		}



		[Fact]
		public async Task Handle_UpdatesAPromptWithNovelAiScenarioSetToNull_WhenNovelAiScenarioIsNull()
		{
			//arrange
			const string? expectedString = null;
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, OwnerId = user.Id, NovelAiScenario = null};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actualPrompt = await DbContext.Prompts.FindAsync(prompt.Id);
			Assert.Equal(expectedString, actualPrompt.NovelAiScenario);
		}

		[Fact]
		public async Task Handle_UpdatesAPromptWithNovelAiScenarioSetToNull_WhenNovelAiScenarioIsEmptyString()
		{
			//arrange
			const string? expectedString = null;
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, OwnerId = user.Id, NovelAiScenario = ""};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actualPrompt = await DbContext.Prompts.FindAsync(prompt.Id);
			Assert.Equal(expectedString, actualPrompt.NovelAiScenario);
		}

		[Fact]
		public async Task Handle_UpdatesAPromptWithNovelAiScenarioSetToNull_WhenNovelAiScenarioIsWhitespace()
		{
			//arrange
			const string? expectedString = null;
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, OwnerId = user.Id, NovelAiScenario = "   "};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actualPrompt = await DbContext.Prompts.FindAsync(prompt.Id);
			Assert.Equal(expectedString, actualPrompt.NovelAiScenario);
		}

		[Fact]
		public async Task Handle_UpdatesAPromptWithNovelAiScenarioSetToExpectedValue()
		{
			//arrange
			const string? expectedString = null;
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, OwnerId = user.Id, NovelAiScenario = expectedString};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actualPrompt = await DbContext.Prompts.FindAsync(prompt.Id);
			Assert.Equal(expectedString, actualPrompt.NovelAiScenario);
		}







		[Fact]
		public async Task Handle_SetsAPromptWithScriptZipSetToNull()
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, OwnerId = user.Id, ScriptZip = null};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actualPrompt = await DbContext.Prompts.FindAsync(prompt.Id);
			Assert.Null(actualPrompt.ScriptZip);
		}

		[Fact]
		public async Task Handle_SetsAPublishDate_WhenSaveDraftIsFalse_AndPromptHasNoPublishDate_AndWasDraft()
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {IsDraft = true, Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, SaveDraft = false};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.NotNull(actual.PublishDate);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task Handle_SetsIsDraftToFalse_WhenSaveDraftIsFalse(bool initialDraftStatus)
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {IsDraft = initialDraftStatus, Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, SaveDraft = false};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.False(actual.IsDraft);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task Handle_SetsIsDraftToFalse_WhenThereIsAParentId(bool intitialStatus)
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var parent = new Prompt {Owner = owner};
			var prompt = new Prompt {Parent = parent, IsDraft = intitialStatus, Owner = owner};
			DbContext.Prompts.Add(parent);
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new UpdatePromptCommand {Id = prompt.Id};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.False(actual.IsDraft);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task Handle_SetsIsDraftToTrue_WhenSaveDraftIsTrue(bool initialDraftStatus)
		{
			//arrange
			var owner = new User {Username = "TestUser"};
			var prompt = new Prompt {IsDraft = initialDraftStatus, Owner = owner};
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel {Id = owner.Id};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand {Id = prompt.Id, SaveDraft = true};

			//act
			await _handler.Handle(command);

			//assert
			Prompt? actual = DbContext.Prompts.Find(prompt.Id);
			Assert.True(actual.IsDraft);
		}
	}
}
