using AIDungeonPrompts.Application.Helpers;
using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.UpdatePrompt
{
	public class UpdatePromptCommandValidator : AbstractValidator<UpdatePromptCommand>
	{
		private const int MAX_SIZE = 500001;

		public UpdatePromptCommandValidator()
		{
			RuleFor(e => e.Id)
				.NotEmpty();
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
				.Must(scriptZip => scriptZip!.Length < MAX_SIZE)
				.WithMessage("File size too large (max 500kb)")
				.When(e => e.ScriptZip != null);
			RuleFor(e => e.ScriptZip)
				.Must(scriptZip => ZipHelper.IsCompressedData(scriptZip!))
				.WithMessage("Please only upload .zip files")
				.DependentRules(() =>
				{
					RuleFor(e => e.ScriptZip)
					.Must(scriptZip => ZipHelper.CheckFileContents(scriptZip!))
					.WithMessage("File was not in the expected format. Please re-export and try again.")
					.When(e => e.ScriptZip != null);
				})
				.When(e => e.ScriptZip?.Length < MAX_SIZE);
		}
	}
}
