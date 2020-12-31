using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetUser;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetUser
{
	public class GetUserQueryValidatorTest
	{
		private readonly GetUserQueryValidator _validator;

		public GetUserQueryValidatorTest()
		{
			_validator = new GetUserQueryValidator();
		}

		[Theory]
		[InlineData(1)]
		[InlineData(34)]
		[InlineData(5264)]
		[InlineData(int.MaxValue)]
		public async Task ValidateAsync_ReturnsValid_WhenTheGivenIdIsNotDefault(int id)
		{
			//arrange
			var query = new GetUserQuery(id);

			//act + assert
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsValidationError_WhenTheGivenIdIsDefault()
		{
			//arrange
			var query = new GetUserQuery(default);

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.False(actual.IsValid);
		}
	}
}
