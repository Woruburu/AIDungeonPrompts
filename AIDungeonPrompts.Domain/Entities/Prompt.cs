using System;
using System.Collections.Generic;
using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class Prompt : BaseDomainEntity
	{
		public string? AuthorsNote { get; set; }
		public List<Prompt> Children { get; set; } = new List<Prompt>();
		public string? Description { get; set; }
		public bool IsDraft { get; set; }
		public string? Memory { get; set; }
		public bool Nsfw { get; set; }
		public User? Owner { get; set; }
		public int? OwnerId { get; set; }
		public Prompt? Parent { get; set; }
		public int? ParentId { get; set; }
		public string PromptContent { get; set; } = string.Empty;
		public List<PromptTag> PromptTags { get; set; } = new List<PromptTag>();
		public DateTime? PublishDate { get; set; }
		public string? Quests { get; set; }
		public List<Report> Reports { get; set; } = new List<Report>();
		public byte[]? ScriptZip { get; set; }
		public string Title { get; set; } = string.Empty;
		public int Upvote { get; set; }
		public int Views { get; set; }
		public List<WorldInfo> WorldInfos { get; set; } = new List<WorldInfo>();

		public string? NovelAiScenario { get; set; }
		public string? HoloAiScenario { get; set; }
	}
}
