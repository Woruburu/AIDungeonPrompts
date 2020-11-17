using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;

namespace AIDungeonPrompts.Web.Models
{
	public class CreatePromptViewModel
	{
		public CreatePromptCommand Command { get; set; } = new CreatePromptCommand();
		public SimilarPromptViewModel SimilarPromptQuery { get; set; } = new SimilarPromptViewModel();
	}
}
