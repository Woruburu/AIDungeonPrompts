using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.CreatePrompt
{
	public class CreatePromptCommandValidator : AbstractValidator<CreatePromptCommand>
	{
		public CreatePromptCommandValidator()
		{
			RuleFor(e => e.PromptContent).NotEmpty();
			RuleFor(e => e.PromptTags).NotEmpty();
			//RuleFor(e => e.Title).NotEmpty();
		}
	}
}
