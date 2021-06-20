using FluentValidation;

namespace AIDungeonPrompts.Application.Queries.LogIn
{
	public class LogInQueryValidator : AbstractValidator<LogInQuery>
	{
		public LogInQueryValidator()
		{
			RuleFor(e => e.Password).NotEmpty();
			RuleFor(e => e.Username).NotEmpty();
		}
	}
}
