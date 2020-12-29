using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.ClearReport
{
	public class ClearReportCommandValidator : AbstractValidator<ClearReportCommand>
	{
		public ClearReportCommandValidator()
		{
			RuleFor(e => e.Id).NotEmpty();
		}
	}
}
