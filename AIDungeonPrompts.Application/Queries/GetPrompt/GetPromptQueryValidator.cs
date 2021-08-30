using FluentValidation;

namespace AIDungeonPrompts.Application.Queries.GetPrompt
{
	public class GetPromptQueryValidator : AbstractValidator<GetPromptQuery>
	{
		public GetPromptQueryValidator()
		{
			RuleFor(e => e.Id).NotEmpty();
		}
	}
}
