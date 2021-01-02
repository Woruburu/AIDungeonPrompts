using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.SimilarPrompt
{
	public class SimilarPromptQueryValidatorTest
	{
		private readonly SimilarPromptQueryValidator _validator;

		public SimilarPromptQueryValidatorTest()
		{
			_validator = new SimilarPromptQueryValidator();
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("	")]
		public async Task ValidateAsync_ReturnsNotValid_WhenTitleIsEmpty(string title)
		{
			//arrange
			var query = new SimilarPromptQuery(title);

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.False(actual.IsValid);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("	")]
		public async Task ValidateAsync_ReturnsNotValid_WhenTitleIsEmpty_AndIdIsDefault(string title)
		{
			//arrange
			var query = new SimilarPromptQuery(title, default);

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsValid_WhenTitleHasValue()
		{
			//arrange
			var query = new SimilarPromptQuery("Value");

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(256)]
		public async Task ValidateAsync_ReturnsValid_WhenTitleHasValue_AndIdIsHasValue(int id)
		{
			//arrange
			var query = new SimilarPromptQuery("Value", id);

			//act
			var actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
