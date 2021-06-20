using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.CreateUser
{
	public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
	{
		public CreateUserCommandValidator()
		{
			RuleFor(e => e.Password).NotEmpty();
			RuleFor(e => e.Username).NotEmpty();
		}
	}
}
