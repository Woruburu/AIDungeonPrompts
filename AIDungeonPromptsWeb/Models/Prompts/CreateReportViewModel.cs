using AIDungeonPrompts.Application.Commands.CreateReport;
using AIDungeonPrompts.Application.Queries.GetPrompt;

namespace AIDungeonPrompts.Web.Models.Prompts
{
	public class CreateReportViewModel
	{
		public CreateReportCommand Command { get; set; } = new();
		public GetPromptViewModel Prompt { get; set; } = new();
	}
}
