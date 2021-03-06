using System.IO;
using System.IO.Compression;
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
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenScriptZipHasValidZipHeaders_AndFiveFiles_ButNotTheExpectedFilenames()
		{
			//arrange
			var command = CreateValidCommand();
			using (var memoryStream = new MemoryStream())
			{
				using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create))
				{
					for (var i = 0; i < 5; i++)
					{
						zip.CreateEntry(i.ToString());
					}
				}
				command.ScriptZip = memoryStream.ToArray();
			}

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenScriptZipHasValidZipHeaders_AndNoOtherContents()
		{
			//arrange
			var command = CreateValidCommand();
			command.ScriptZip = new byte[] { 0x50, 0x4b, 0x03, 0x04 };

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.False(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidFalse_WhenScriptZipIsAnEmptyArray()
		{
			//arrange
			var command = CreateValidCommand();
			command.ScriptZip = new byte[] { };

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
		public async Task ValidateAsync_ReturnsIsValidTrue_WhenScriptZipHasValidZipHeaders_AndFiveFiles_AndTheExpectedFilenames()
		{
			//arrange
			var command = CreateValidCommand();
			var expectedFiles = new string[] { "contextModifier.js", "inputModifier.js", "outputModifier.js", "shared.js" };
			using (var memoryStream = new MemoryStream())
			{
				using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create))
				{
					zip.CreateEntry("DefaultFolder");
					foreach (var file in expectedFiles)
					{
						zip.CreateEntry(file);
					}
				}
				command.ScriptZip = memoryStream.ToArray();
			}

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.True(actual.IsValid);
		}

		[Fact]
		public async Task ValidateAsync_ReturnsIsValidTrue_WhenScriptZipHasValidZipHeaders_AndTheExpectedFilenames()
		{
			//arrange
			var command = CreateValidCommand();
			var expectedFiles = new string[] { "contextModifier.js", "inputModifier.js", "outputModifier.js", "shared.js" };
			using (var memoryStream = new MemoryStream())
			{
				using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create))
				{
					foreach (var file in expectedFiles)
					{
						zip.CreateEntry(file);
					}
				}
				command.ScriptZip = memoryStream.ToArray();
			}

			//act
			var actual = await _validator.ValidateAsync(command);

			//assert
			Assert.True(actual.IsValid);
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

		private CreatePromptCommand CreateValidCommand()
		{
			return new CreatePromptCommand()
			{
				Title = "NewTitle",
				PromptContent = "PromptContent",
				ParentId = 1
			};
		}
	}
}
