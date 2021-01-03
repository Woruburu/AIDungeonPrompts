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
			var owner = new User { Username = "TestUser" };
			var prompt = new Prompt { IsDraft = expectedDraftStatus, Owner = owner };
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel { Id = int.MaxValue, Role = RoleEnum.FieldEdit };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand { Id = prompt.Id, SaveDraft = !expectedDraftStatus };

			//act
			await _handler.Handle(command);

			//assert
			var actual = DbContext.Prompts.Find(prompt.Id);
			Assert.Equal(expectedDraftStatus, actual.IsDraft);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task Handle_SetsIsDraftToFalse_WhenSaveDraftIsFalse(bool initialDraftStatus)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var prompt = new Prompt { IsDraft = initialDraftStatus, Owner = owner };
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand { Id = prompt.Id, SaveDraft = false };

			//act
			await _handler.Handle(command);

			//assert
			var actual = DbContext.Prompts.Find(prompt.Id);
			Assert.False(actual.IsDraft);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task Handle_SetsIsDraftToTrue_WhenSaveDraftIsTrue(bool initialDraftStatus)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var prompt = new Prompt { IsDraft = initialDraftStatus, Owner = owner };
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new UpdatePromptCommand { Id = prompt.Id, SaveDraft = true };

			//act
			await _handler.Handle(command);

			//assert
			var actual = DbContext.Prompts.Find(prompt.Id);
			Assert.True(actual.IsDraft);
		}
	}
}
