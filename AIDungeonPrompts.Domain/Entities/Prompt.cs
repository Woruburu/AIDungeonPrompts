using System.Collections.Generic;
using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class Prompt : BaseDomainEntity
	{
		public string? AuthorsNote { get; set; }
		public string? Memory { get; set; }
		public string? Description { get; set; }
		public bool Nsfw { get; set; }
		public string PromptContent { get; set; } = string.Empty;
		public List<PromptTag> PromptTags { get; set; } = new List<PromptTag>();
		public string? Quests { get; set; }
		public string Title { get; set; } = string.Empty;
		public int Upvote { get; set; }
		public int Views { get; set; }
		public List<WorldInfo> WorldInfos { get; set; } = new List<WorldInfo>();
	}
}
