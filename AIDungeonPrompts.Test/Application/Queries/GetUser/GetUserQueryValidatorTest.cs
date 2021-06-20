using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetUser;
using FluentValidation.Results;
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

		[Fact]
		public async Task ValidateAsync_ReturnsNotValid_WhenTheGivenIdIsDefault()
		{
			//arrange
			var query = new GetUserQuery(default);

			//act
			ValidationResult? actual = await _validator.ValidateAsync(query);

			//assert
			Assert.False(actual.IsValid);
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
			ValidationResult? actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
