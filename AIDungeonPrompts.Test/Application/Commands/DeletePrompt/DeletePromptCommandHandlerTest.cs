using System.Collections.Generic;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.DeletePrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using AIDungeonPrompts.Test.Collections.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.DeletePrompt
{
	public class DeletePromptCommandHandlerTest : DatabaseFixtureTest
	{
		private readonly DeletePromptCommandHandler _handler;
		private readonly Mock<ICurrentUserService> _mockUserService;

		public DeletePromptCommandHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_mockUserService = new Mock<ICurrentUserService>();
			_handler = new DeletePromptCommandHandler(DbContext, _mockUserService.Object);
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(1, 10)]
		[InlineData(12, 20)]
		[InlineData(64, 64)]
		public async Task Handle_RemovesPrompt_AndAllItsChildren_AndAllSubChildren_WhenUserHasDeletePermission(int expectedPromptCount, int childDepth)
		{
			//arrange
			var child = new Prompt();
			var prompt = new Prompt
			{
				Children = new List<Prompt> { child }
			};
			for (var i = 0; i < childDepth; i++)
			{
				var newChild = new Prompt();
				child.Children.Add(newChild);
				child = newChild;
			}
			DbContext.Add(prompt);
			for (var i = 0; i < expectedPromptCount; i++)
			{
				DbContext.Add(new Prompt());
			}
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Role = RoleEnum.Delete };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.Equal(expectedPromptCount, await DbContext.Prompts.CountAsync());
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(1, 10)]
		[InlineData(12, 20)]
		[InlineData(64, 64)]
		public async Task Handle_RemovesPrompt_AndAllItsChildren_AndAllSubChildren_WhenUserIsOwner(int expectedPromptCount, int childDepth)
		{
			//arrange
			var owner = new User { Username = "TestOwner" };
			var child = new Prompt();
			var prompt = new Prompt
			{
				Children = new List<Prompt> { child },
				Owner = owner
			};
			for (var i = 0; i < childDepth; i++)
			{
				var newChild = new Prompt();
				child.Children.Add(newChild);
				child = newChild;
			}
			DbContext.Add(prompt);
			for (var i = 0; i < expectedPromptCount; i++)
			{
				DbContext.Add(new Prompt());
			}
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.Equal(expectedPromptCount, await DbContext.Prompts.CountAsync());
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(1, 10)]
		[InlineData(12, 20)]
		[InlineData(64, 64)]
		public async Task Handle_RemovesPrompt_AndAllItsChildren_WhenUserHasDeletePermission(int expectedPromptCount, int childAmount)
		{
			//arrange
			var prompt = new Prompt();
			for (var i = 0; i < childAmount; i++)
			{
				prompt.Children.Add(new Prompt());
			}
			DbContext.Add(prompt);
			for (var i = 0; i < expectedPromptCount; i++)
			{
				DbContext.Add(new Prompt());
			}
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Role = RoleEnum.Delete };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.Equal(expectedPromptCount, await DbContext.Prompts.CountAsync());
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(1, 10)]
		[InlineData(12, 20)]
		[InlineData(64, 64)]
		public async Task Handle_RemovesPrompt_AndAllItsChildren_WhenUserIsOwner(int expectedPromptCount, int childAmount)
		{
			//arrange
			var owner = new User { Username = "TestOwner" };
			var prompt = new Prompt
			{
				Owner = owner
			};
			for (var i = 0; i < childAmount; i++)
			{
				prompt.Children.Add(new Prompt());
			}
			DbContext.Add(prompt);
			for (var i = 0; i < expectedPromptCount; i++)
			{
				DbContext.Add(new Prompt());
			}
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.Equal(expectedPromptCount, await DbContext.Prompts.CountAsync());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(12)]
		[InlineData(64)]
		public async Task Handle_RemovesPrompt_WhenUserHasDeletePermission(int expectedPromptCount)
		{
			//arrange
			var prompt = new Prompt();
			DbContext.Add(prompt);
			for (var i = 0; i < expectedPromptCount; i++)
			{
				DbContext.Add(new Prompt());
			}
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Role = RoleEnum.Delete };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.Equal(expectedPromptCount, await DbContext.Prompts.CountAsync());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(12)]
		[InlineData(64)]
		public async Task Handle_RemovesPrompt_WhenUserIsOwner(int expectedPromptCount)
		{
			//arrange
			var owner = new User { Username = "TestOwner" };
			var prompt = new Prompt
			{
				Owner = owner
			};
			DbContext.Add(prompt);
			for (var i = 0; i < expectedPromptCount; i++)
			{
				var extraPrompt = new Prompt();
				DbContext.Add(extraPrompt);
			}
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.Equal(expectedPromptCount, await DbContext.Prompts.CountAsync());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(-10)]
		[InlineData(64)]
		[InlineData(128)]
		public async Task Handle_ThrowsDeletePromptDoesNotExistException_WhenPromptIdDoesNotExist(int id)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			DbContext.Users.Add(owner);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel { Id = owner.Id };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var command = new DeletePromptCommand(id);

			//act + assert
			await Assert.ThrowsAsync<DeletePromptDoesNotExistException>(async () => await _handler.Handle(command));
		}

		[Theory]
		[InlineData(10)]
		[InlineData(100)]
		[InlineData(-10)]
		[InlineData(256)]
		public async Task Handle_ThrowsDeletePromptUserUnauthorizedException_WhenThePromptHasNoOwner(int userId)
		{
			//arrange
			var prompt = new Prompt();
			DbContext.Add(prompt);
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Id = userId };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act + assert
			await Assert.ThrowsAsync<DeletePromptUserUnauthorizedException>(async () => await _handler.Handle(command));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(-10)]
		[InlineData(64)]
		[InlineData(128)]
		public async Task Handle_ThrowsDeletePromptUserUnauthorizedException_WhenThereIsNoUserLoggedIn(int id)
		{
			//arrange
			var command = new DeletePromptCommand(id);

			//act + assert
			await Assert.ThrowsAsync<DeletePromptUserUnauthorizedException>(async () => await _handler.Handle(command));
		}

		[Theory]
		[InlineData(10)]
		[InlineData(100)]
		[InlineData(-10)]
		[InlineData(256)]
		public async Task Handle_ThrowsDeletePromptUserUnauthorizedException_WhenTheUserIdDoesNotMatchPromptOwnerId(int userId)
		{
			//arrange
			var owner = new User { Username = "TestOwner" };
			var prompt = new Prompt
			{
				Owner = owner
			};
			DbContext.Add(prompt);
			await DbContext.SaveChangesAsync();

			var user = new GetUserViewModel { Id = userId };
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);

			var command = new DeletePromptCommand(prompt.Id);

			//act + assert
			await Assert.ThrowsAsync<DeletePromptUserUnauthorizedException>(async () => await _handler.Handle(command));
		}
	}
}
