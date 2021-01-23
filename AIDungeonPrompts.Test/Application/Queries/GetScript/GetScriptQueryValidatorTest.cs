using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetScript;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetScript
{
	public class GetScriptQueryValidatorTest
	{
		private readonly GetScriptQueryValidator _validator;

		public GetScriptQueryValidatorTest()
		{
			_validator = new GetScriptQueryValidator();
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenIdIsDefault()
		{
			//arrange
			var query = new GetScriptQuery(default);

			//act
			var results = await _validator.ValidateAsync(query);

			//assert
			Assert.False(results.IsValid);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(64)]
		[InlineData(256)]
		public async Task ValidateAsync_ReturnsIsValidTrue_WhenIdIsNotDefault(int id)
		{
			//arrange
			var query = new GetScriptQuery(id);

			//act
			var results = await _validator.ValidateAsync(query);

			//assert
			Assert.True(results.IsValid);
		}
	}
}
