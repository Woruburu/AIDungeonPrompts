using FluentValidation;

namespace AIDungeonPrompts.Application.Queries.GetScript
{
	public class GetScriptQueryValidator : AbstractValidator<GetScriptQuery>
	{
		public GetScriptQueryValidator()
		{
			RuleFor(e => e.PromptId).NotEmpty();
		}
	}
}
