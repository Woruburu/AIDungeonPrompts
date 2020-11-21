using FluentValidation;
using MediatR;

namespace AIDungeonPrompts.Application.Commands.UpdateUser
{

	public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
	{
		public UpdateUserCommandValidator()
		{
			RuleFor(e => e.Id).NotEmpty();
			RuleFor(e => e.Username).NotEmpty();
			RuleFor(e => e.Password).NotEmpty();
		}
	}
}
