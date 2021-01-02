using FluentValidation;

namespace AIDungeonPrompts.Application.Queries.GetUser
{
	public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
	{
		public GetUserQueryValidator()
		{
			RuleFor(e => e.Id).NotEmpty();
		}
	}
}
