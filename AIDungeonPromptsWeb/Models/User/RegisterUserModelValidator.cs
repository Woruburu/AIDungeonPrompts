using FluentValidation;

namespace AIDungeonPrompts.Web.Models.User
{
	public class RegisterUserModelValidator : AbstractValidator<RegisterUserModel>
	{
		public RegisterUserModelValidator()
		{
			RuleFor(e => e.Password).NotEmpty();
			RuleFor(e => e.Username).NotEmpty();
		}
	}
}
