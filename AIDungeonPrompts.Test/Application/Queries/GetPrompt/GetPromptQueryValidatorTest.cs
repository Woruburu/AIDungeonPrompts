using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetPrompt
{
	public class GetPromptQueryValidatorTest
	{
		private readonly GetPromptQueryValidator _validator;

		public GetPromptQueryValidatorTest()
		{
			_validator = new GetPromptQueryValidator();
		}

		[Fact]
		public async Task ValidateAsync_ReturnsNotValid_WhenTheGivenIdIsDefault()
		{
			//arrange
			var query = new GetPromptQuery(default);

			//act
			var actual = await _validator.ValidateAsync(query);

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
			var query = new GetPromptQuery(id);

			//act + assert
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
