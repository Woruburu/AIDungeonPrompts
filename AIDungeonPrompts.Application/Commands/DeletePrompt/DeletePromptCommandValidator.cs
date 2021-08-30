using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.DeletePrompt
{
	public class DeletePromptCommandValidator : AbstractValidator<DeletePromptCommand>
	{
		public DeletePromptCommandValidator()
		{
			RuleFor(e => e.Id).NotEmpty();
		}
	}
}
