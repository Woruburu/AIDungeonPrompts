using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.CreatePrompt
{
	public class CreatePromptCommandValidatorTest
	{
		private readonly CreatePromptCommandValidator _validator;

		public CreatePromptCommandValidatorTest()
		{
			_validator = new CreatePromptCommandValidator();
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenCommandIsEmpty()
		{
			//arrange
			var command = new CreatePromptCommand();

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenTitleContentAndTagsAreFilledIn_AndAParentIdIsNull()
		{
			//arrange
			var command = new CreatePromptCommand()
			{
				Title = "NewTitle",
				PromptContent = "PromptContent",
				ParentId = null
			};

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidTrue_WhenTitleContentAndTagsAreFilledIn()
		{
			//arrange
			var command = new CreatePromptCommand()
			{
				Title = "NewTitle",
				PromptContent = "PromptContent",
				PromptTags = "tag"
			};

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.True(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidTrue_WhenTitleContentAndTagsAreFilledIn_AndHasAParentId()
		{
			//arrange
			var command = new CreatePromptCommand()
			{
				Title = "NewTitle",
				PromptContent = "PromptContent",
				ParentId = 1
			};

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
