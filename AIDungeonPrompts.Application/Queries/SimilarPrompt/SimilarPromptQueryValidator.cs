using FluentValidation;

namespace AIDungeonPrompts.Application.Queries.SimilarPrompt
{
	public class SimilarPromptQueryValidator : AbstractValidator<SimilarPromptQuery>
	{
		public SimilarPromptQueryValidator()
		{
			RuleFor(e => e.Title).NotEmpty();
		}
	}
}
