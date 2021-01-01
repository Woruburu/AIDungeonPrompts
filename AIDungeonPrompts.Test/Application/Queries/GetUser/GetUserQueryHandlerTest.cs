using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetUser
{
	public class GetUserQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly GetUserQueryHandler _handler;

		public GetUserQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new GetUserQueryHandler(DbContext);
		}

		[Fact]
		public async Task Handle_ReturnsNull_WhenDatabaseIsEmpty()
		{
			//arrange
			var query = new GetUserQuery(int.MaxValue);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Null(actual);
		}

		[Fact]
		public async Task Handle_ReturnsNull_WhenUserDoesntExist()
		{
			//arrange
			DbContext.Users.AddRange(
				new User { Username = "1" },
				new User { Username = "2" },
				new User { Username = "3" },
				new User { Username = "4" });
			await DbContext.SaveChangesAsync();
			var query = new GetUserQuery(int.MaxValue);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Null(actual);
		}

		[Theory]
		[InlineData(1, 0)]
		[InlineData(3, 1)]
		[InlineData(10, 4)]
		[InlineData(200, 56)]
		public async Task Handle_ReturnsUser(int amount, int index)
		{
			//arrange
			const string expectedUsername = "TestUsername";
			var user = new User();
			for (var i = 0; i < amount; i++)
			{
				if (i == index)
				{
					user = new User() { Username = expectedUsername };
					DbContext.Users.Add(user);
					await DbContext.SaveChangesAsync();
				}
				else
				{
					DbContext.Users.Add(new User() { Username = i.ToString() });
				}
			}

			await DbContext.SaveChangesAsync();
			var query = new GetUserQuery(user.Id);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(user.Id, actual.Id);
			Assert.Equal(expectedUsername, actual.Username);
		}
	}
}
