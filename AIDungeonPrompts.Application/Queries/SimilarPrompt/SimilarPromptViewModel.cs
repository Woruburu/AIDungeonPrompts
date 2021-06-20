using System.Collections.Generic;

namespace AIDungeonPrompts.Application.Queries.SimilarPrompt
{
	public class SimilarPromptViewModel
	{
		public bool Matched { get; set; }

		public List<SimilarPromptDetailsViewModel> SimilarPrompts { get; set; } =
			new List<SimilarPromptDetailsViewModel>();
	}
}
