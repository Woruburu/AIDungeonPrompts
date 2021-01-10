using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.UpdatePrompt
{
	public class UpdatePromptCommandValidatorTest
	{
		private readonly UpdatePromptCommandValidator _validator;

		public UpdatePromptCommandValidatorTest()
		{
			_validator = new UpdatePromptCommandValidator();
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenCommandIsEmpty()
		{
			//arrange
			var command = new UpdatePromptCommand();

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenTitleContentAndTagsAreFilledIn_AndAParentIdIsNull()
		{
			//arrange
			var command = new UpdatePromptCommand()
			{
				Id = 1,
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
			var command = new UpdatePromptCommand()
			{
				Id = 1,
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
			var command = new UpdatePromptCommand()
			{
				Id = 1,
				Title = "NewTitle",
				PromptContent = "PromptContent",
				ParentId = 2
			};

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}
