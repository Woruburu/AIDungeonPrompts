using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.UpdatePrompt
{
	public class UpdatePromptCommandValidator : AbstractValidator<UpdatePromptCommand>
	{
		public UpdatePromptCommandValidator()
		{
			RuleFor(e => e.PromptContent)
				.NotEmpty()
				.WithMessage("Please supply a Prompt");
			RuleFor(e => e.PromptTags)
				.NotEmpty()
				.WithMessage("Please supply at least a single tag");
			RuleFor(e => e.Title)
				.NotEmpty()
				.WithMessage("Please supply a Title");
		}
	}
}
