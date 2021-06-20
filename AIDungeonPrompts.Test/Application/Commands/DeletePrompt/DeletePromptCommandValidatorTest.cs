using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.DeletePrompt;
using FluentValidation.Results;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.DeletePrompt
{
	public class DeletePromptCommandValidatorTest
	{
		private readonly DeletePromptCommandValidator _validator;

		public DeletePromptCommandValidatorTest()
		{
			_validator = new DeletePromptCommandValidator();
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenIdIsDefault()
		{
			//arrange
			var command = new DeletePromptCommand(default);

			//act
			ValidationResult? actual = await _validator.ValidateAsync(command);

			//assert
			Assert.False(actual.IsValid);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(64)]
		[InlineData(128)]
		public async Task ValidateAsync_ReturnsIsValidTrue_WhenIdHasValue(int id)
		{
			//arrange
			var command = new DeletePromptCommand(id);

			//act
			ValidationResult? actual = await _validator.ValidateAsync(command);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
