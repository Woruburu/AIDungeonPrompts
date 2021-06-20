using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using Microsoft.AspNetCore.Http;

namespace AIDungeonPrompts.Web.Models.Prompts
{
	public class CreatePromptViewModel
	{
		public CreatePromptCommand Command { get; set; } = new();
		public IFormFile? ScriptZip { get; set; }
		public SimilarPromptViewModel SimilarPromptQuery { get; set; } = new();
		public IFormFile? WorldInfoFile { get; set; }
		public string? DisabledMessage { get; set; }
		public bool CreationDisabled { get; set; }
	}
}
