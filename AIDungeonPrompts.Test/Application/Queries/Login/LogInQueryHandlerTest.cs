using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.LogIn;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.Login
{
	public class LogInQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly LogInQueryHandler _handler;

		public LogInQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new LogInQueryHandler(DbContext);
		}

		[Theory]
		[InlineData(1, 0, RoleEnum.Delete)]
		[InlineData(3, 1, RoleEnum.TagEdit)]
		[InlineData(10, 4, RoleEnum.None)]
		[InlineData(200, 56, RoleEnum.FieldEdit)]
		public async Task Handle_ReturnsUser(int amount, int index, RoleEnum expectedRole)
		{
			//arrange
			const string expectedUsername = "TestUsername";
			const string password = "TestPassword";
			var user = new User();
			for (var i = 0; i < amount; i++)
			{
				if (i == index)
				{
					user = new User()
					{
						Username = expectedUsername,
						Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 4),
						Role = expectedRole
					};
					DbContext.Users.Add(user);
					await DbContext.SaveChangesAsync();
				}
				else
				{
					DbContext.Users.Add(new User
					{
						Username = i.ToString(),
						Password = BCrypt.Net.BCrypt.EnhancedHashPassword(i.ToString(), 4)
					});
				}
			}
			await DbContext.SaveChangesAsync();
			var query = new LogInQuery(expectedUsername, password);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(user.Id, actual.Id);
			Assert.Equal(expectedUsername, actual.Username);
			Assert.Equal(expectedRole, actual.Role);
		}

		[Fact]
		public async Task Handle_ReturnsUser_WhenUsernameCasesDontMatch()
		{
			//arrange
			const string expectedUsername = "testusername";
			const RoleEnum expectedRole = default;
			const string password = "TestPassword";
			var user = new User
			{
				Username = expectedUsername,
				Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 4),
				Role = expectedRole
			};
			DbContext.Users.Add(user);
			await DbContext.SaveChangesAsync();
			var query = new LogInQuery(expectedUsername.ToUpper(), password);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(user.Id, actual.Id);
			Assert.Equal(expectedUsername, actual.Username);
			Assert.Equal(expectedRole, actual.Role);
		}

		[Fact]
		public async Task Handle_ThrowsLoginFailedException_WhenPasswordsDoNotMatch()
		{
			//arrange
			const string username = "TestUsername";
			const string password = "TestPassword";
			DbContext.Users.Add(new User
			{
				Username = username,
				Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password)
			});
			await DbContext.SaveChangesAsync();
			var query = new LogInQuery(username, "WrongPassword");

			//act + assert
			await Assert.ThrowsAsync<LoginFailedException>(async () =>
			{
				await _handler.Handle(query);
			});
		}

		[Fact]
		public async Task Handle_ThrowsLoginFailedException_WhenThereIsNoMatchingUsernameInTheDatabase()
		{
			//arrange
			const string username = "TestUsername";
			DbContext.Users.Add(new User { Username = username });
			await DbContext.SaveChangesAsync();
			var query = new LogInQuery(username, "TestPassword");

			//act + assert
			await Assert.ThrowsAsync<LoginFailedException>(async () =>
			{
				await _handler.Handle(query);
			});
		}

		[Fact]
		public async Task Handle_ThrowsLoginFailedException_WhenThereIsNoPasswordAgainstTheUsername()
		{
			//arrange
			var query = new LogInQuery("TestUsername", "TestPassword");

			//act + assert
			await Assert.ThrowsAsync<LoginFailedException>(async () =>
			{
				await _handler.Handle(query);
			});
		}
	}
}
