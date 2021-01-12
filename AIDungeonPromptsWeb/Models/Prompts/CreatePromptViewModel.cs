using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using Microsoft.AspNetCore.Http;

namespace AIDungeonPrompts.Web.Models.Prompts
{
	public class CreatePromptViewModel
	{
		public CreatePromptCommand Command { get; set; } = new CreatePromptCommand();
		public SimilarPromptViewModel SimilarPromptQuery { get; set; } = new SimilarPromptViewModel();
		public IFormFile? WorldInfoFile { get; set; }
	}
}
