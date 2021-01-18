using AIDungeonPrompts.Application.Helpers;
using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.CreatePrompt
{
	public class CreatePromptCommandValidator : AbstractValidator<CreatePromptCommand>
	{
		public CreatePromptCommandValidator()
		{
			RuleFor(e => e.PromptContent)
				.NotEmpty()
				.WithMessage("Please supply a Prompt");
			RuleFor(e => e.PromptTags)
				.NotEmpty()
				.WithMessage("Please supply at least a single tag")
				.When(e => !e.ParentId.HasValue);
			RuleFor(e => e.Title)
				.NotEmpty()
				.WithMessage("Please supply a Title");
			RuleFor(e => e.ScriptZip)
				.Must(scriptZip => scriptZip!.Length < 5000000)
				.WithMessage("File size too large (max 500kb)")
				.When(e => e.ScriptZip != null);
			RuleFor(e => e.ScriptZip)
				.Must(scriptZip => ZipHelper.IsCompressedData(scriptZip!))
				.WithMessage("Please only upload .zip files")
				.When(e => e.ScriptZip != null);
		}
	}
}
