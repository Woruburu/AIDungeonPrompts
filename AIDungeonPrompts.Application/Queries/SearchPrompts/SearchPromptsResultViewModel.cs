using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.SearchPrompts
{
	public class SearchPromptsResultViewModel
	{
		public DateTime DateCreated { get; set; }
		public string? Description { get; set; }
		public int Id { get; set; }

		[Display(Name = "NSFW?")]
		public bool Nsfw { get; set; }

		[Display(Name = "Prompt")]
		public string PromptContent { get; set; } = string.Empty;

		[Display(Name = "Tags")]
		public IEnumerable<SearchPromptsTagViewModel> SearchPromptsTagViewModel { get; set; } = new List<SearchPromptsTagViewModel>();

		public string Title { get; set; } = string.Empty;
	}
}
