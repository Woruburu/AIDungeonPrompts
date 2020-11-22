using AIDungeonPrompts.Application.Queries.GetPrompt;

namespace AIDungeonPrompts.Web.Models.Prompts
{
	public class ViewPromptViewModel
	{
		public GetPromptViewModel Prompt { get; set; } = new GetPromptViewModel();
		public bool? Reported { get; set; }
	}
}
