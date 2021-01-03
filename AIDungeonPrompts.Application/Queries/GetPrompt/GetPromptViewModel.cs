using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.GetPrompt
{
	public class GetPromptViewModel
	{
		[Display(Name = "Author's Note")]
		public string? AuthorsNote { get; set; }

		public DateTime DateCreated { get; set; }
		public string? Description { get; set; }
		public int Id { get; set; }
		public bool IsDraft { get; set; }
		public string? Memory { get; set; }

		[Display(Name = "NSFW?")]
		public bool Nsfw { get; set; }

		public int? OwnerId { get; set; }

		[Display(Name = "Prompt")]
		public string PromptContent { get; set; } = string.Empty;

		[Display(Name = "Tags")]
		public IEnumerable<GetPromptPromptTagViewModel> PromptTags { get; set; } = new List<GetPromptPromptTagViewModel>();

		public string? Quests { get; set; }
		public string Title { get; set; } = string.Empty;

		[Display(Name = "World Info")]
		public IEnumerable<GetPromptWorldInfoViewModel> WorldInfos { get; set; } = new List<GetPromptWorldInfoViewModel>();
	}
}
