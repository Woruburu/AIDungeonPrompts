using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.UpdatePrompt
{
	public class UpdatePromptCommandValidator : AbstractValidator<UpdatePromptCommand>
	{
		public UpdatePromptCommandValidator()
		{
			RuleFor(e => e.PromptContent).NotEmpty();
			RuleFor(e => e.PromptTags).NotEmpty();
			RuleFor(e => e.Title).NotEmpty();
		}
	}
}
