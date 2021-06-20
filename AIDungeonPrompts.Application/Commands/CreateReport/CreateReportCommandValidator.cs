using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.CreateReport
{
	public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
	{
		public CreateReportCommandValidator()
		{
			RuleFor(e => e.PromptId).NotEmpty();
		}
	}
}
