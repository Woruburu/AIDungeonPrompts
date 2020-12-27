using FluentValidation;

namespace AIDungeonPrompts.Application.Commands.UpdateUser
{
	public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
	{
		public UpdateUserCommandValidator()
		{
			RuleFor(e => e.Id).NotEmpty();
		}
	}
}
