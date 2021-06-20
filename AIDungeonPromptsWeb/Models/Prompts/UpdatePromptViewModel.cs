using System.Collections.Generic;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using Microsoft.AspNetCore.Http;

namespace AIDungeonPrompts.Web.Models.Prompts
{
	public class UpdatePromptViewModel
	{
		public List<GetPromptChild> Children { get; set; } = new();
		public UpdatePromptCommand Command { get; set; } = new();
		public IFormFile? ScriptZip { get; set; }
		public SimilarPromptViewModel SimilarPromptQuery { get; set; } = new();
		public IFormFile? WorldInfoFile { get; set; }
	}
}
