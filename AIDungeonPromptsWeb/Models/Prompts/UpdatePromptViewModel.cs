using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;

namespace AIDungeonPrompts.Web.Models.Prompts
{
	public class UpdatePromptViewModel
	{
		public UpdatePromptCommand Command { get; set; } = new UpdatePromptCommand();
		public SimilarPromptViewModel SimilarPromptQuery { get; set; } = new SimilarPromptViewModel();
	}
}
