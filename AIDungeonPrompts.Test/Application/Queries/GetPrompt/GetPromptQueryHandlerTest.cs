using System.Linq;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Moq;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetPrompt
{
	public class GetPromptQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly GetPromptQueryHandler _handler;
		private readonly Mock<ICurrentUserService> _mockUserService;

		public GetPromptQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_mockUserService = new Mock<ICurrentUserService>();
			_handler = new GetPromptQueryHandler(DbContext, _mockUserService.Object);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(4)]
		[InlineData(12)]
		public async Task Handle_ReturnsAllChildPrompts(int childCount)
		{
			//arrange
			var expectedTitle = "Child";
			var owner = new User { Username = "TestUser" };
			var parent = new Prompt()
			{
				Title = "Parent",
				IsDraft = false,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			for (var i = 0; i < childCount; i++)
			{
				var child = new Prompt
				{
					Title = expectedTitle,
					IsDraft = true,
					Owner = owner,
					Parent = parent
				};
				DbContext.Prompts.Add(child);
			}
			await DbContext.SaveChangesAsync();
			var query = new GetPromptQuery(parent.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.NotNull(result);
			Assert.Equal(childCount, result.Children.Count());
			Assert.All(result.Children, e => string.Equals(expectedTitle, e.Title));
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

		[Fact]
		public async Task Handle_ReturnsNull_IfPromptIsDraft_AndUserServiceDoesNotReturnUser()
		{
			//arrange
			var prompt = new Prompt() { IsDraft = true };
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			GetUserViewModel? user = null;
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(false);
			var query = new GetPromptQuery(prompt.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
		}

		[Fact]
		public async Task Handle_ReturnsNull_IfPromptIsDraft_AndUserServiceReturnsUserWithIncorrectId()
		{
			//arrange
			var prompt = new Prompt() { IsDraft = true, Owner = new User { Username = "TestUser" } };
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel
			{
				Id = int.MaxValue
			};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var query = new GetPromptQuery(prompt.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
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

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(4)]
		[InlineData(12)]
		public async Task Handle_ReturnsNull_WhenTopLevelParentIsDraft_AndUserServiceReturnsNoUser(int depth)
		{
			//arrange
			var owner = new User { Username = "TestUser" };

			var parent = new Prompt()
			{
				IsDraft = true,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var id = parent.Id;
			for (var i = 0; i < depth; i++)
			{
				var child = new Prompt
				{
					IsDraft = false,
					Owner = owner,
					ParentId = id
				};
				DbContext.Prompts.Add(child);
				await DbContext.SaveChangesAsync();
				id = child.Id;
			}

			var query = new GetPromptQuery(id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(4)]
		[InlineData(12)]
		public async Task Handle_ReturnsNull_WhenTopLevelParentIsDraft_AndUserServiceReturnsUserWithWrongId(int depth)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var newUser = new User { Username = "NewUser" };
			DbContext.Users.Add(newUser);

			var parent = new Prompt()
			{
				IsDraft = true,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var id = parent.Id;
			for (var i = 0; i < depth; i++)
			{
				var child = new Prompt
				{
					IsDraft = false,
					Owner = owner,
					ParentId = id
				};
				DbContext.Prompts.Add(child);
				await DbContext.SaveChangesAsync();
				id = child.Id;
			}
			var user = new GetUserViewModel
			{
				Id = newUser.Id
			};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var query = new GetPromptQuery(id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.Null(result);
		}

		[Fact]
		public async Task Handle_ReturnsPrompt_IfPromptIsDraft_AndUserServiceReturnsUserWithCorrectId()
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var prompt = new Prompt() { IsDraft = true, Owner = owner };
			DbContext.Prompts.Add(prompt);
			await DbContext.SaveChangesAsync();
			var user = new GetUserViewModel
			{
				Id = owner.Id
			};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var query = new GetPromptQuery(prompt.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.NotNull(result);
			Assert.True(result.IsDraft);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(4)]
		[InlineData(12)]
		public async Task Handle_ReturnsPrompt_WhenTopLevelParentIsDraft_AndUserServiceReturnsUserWithMatchingId(int depth)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var parent = new Prompt()
			{
				IsDraft = true,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var id = parent.Id;
			for (var i = 0; i < depth; i++)
			{
				var child = new Prompt
				{
					IsDraft = false,
					Owner = owner,
					ParentId = id
				};
				DbContext.Prompts.Add(child);
				await DbContext.SaveChangesAsync();
				id = child.Id;
			}
			var user = new GetUserViewModel
			{
				Id = owner.Id
			};
			_mockUserService.Setup(e => e.TryGetCurrentUser(out user)).Returns(true);
			var query = new GetPromptQuery(id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.NotNull(result);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(4)]
		[InlineData(12)]
		public async Task Handle_ReturnsPrompt_WhenTopLevelParentIsNotDraft(int depth)
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var parent = new Prompt()
			{
				IsDraft = false,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var id = parent.Id;
			for (var i = 0; i < depth; i++)
			{
				var child = new Prompt
				{
					IsDraft = true,
					Owner = owner,
					ParentId = id
				};
				DbContext.Prompts.Add(child);
				await DbContext.SaveChangesAsync();
				id = child.Id;
			}
			var query = new GetPromptQuery(id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task Handle_ReturnsPromptWithNoParentId_WhenPromptDoesNotHaveAParent()
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var parent = new Prompt()
			{
				Title = "Parent",
				IsDraft = false,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			await DbContext.SaveChangesAsync();
			var query = new GetPromptQuery(parent.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.NotNull(result);
			Assert.Null(result.ParentId);
		}

		[Fact]
		public async Task Handle_ReturnsPromptWithParentIdSet_WhenPromptHasAParent()
		{
			//arrange
			var owner = new User { Username = "TestUser" };
			var parent = new Prompt()
			{
				Title = "Parent",
				IsDraft = false,
				Owner = owner
			};
			DbContext.Prompts.Add(parent);
			var child = new Prompt
			{
				IsDraft = true,
				Owner = owner,
				Parent = parent
			};
			DbContext.Prompts.Add(child);
			await DbContext.SaveChangesAsync();
			var query = new GetPromptQuery(child.Id);

			//act
			var result = await _handler.Handle(query);

			//assert
			Assert.NotNull(result);
			Assert.NotNull(result.ParentId);
			Assert.Equal(parent.Id, result.ParentId);
		}
	}
}
