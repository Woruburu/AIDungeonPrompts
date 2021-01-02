using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.LogIn;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.Login
{
	public class LogInQueryValidatorTest
	{
		private readonly LogInQueryValidator _validator;

		public LogInQueryValidatorTest()
		{
			_validator = new LogInQueryValidator();
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("", null)]
		[InlineData("	", null)]
		[InlineData(null, "Value")]
		[InlineData("", "Value")]
		[InlineData("	", "Value")]
		[InlineData("Value", null)]
		[InlineData("Value", "")]
		[InlineData("Value", "	")]
		public async Task ValidateAsync_ReturnsNotValid_WhenEitherPasswordOrUsernameAreEmpty(string username, string password)
		{
			//arrange
			var query = new LogInQuery(username, password);

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsValid_WhenPasswordAndUsernameHaveValue()
		{
			//arrange
			var query = new LogInQuery("Value", "Value");

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
