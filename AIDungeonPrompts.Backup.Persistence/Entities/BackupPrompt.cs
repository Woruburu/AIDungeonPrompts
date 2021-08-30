using System;
using System.Collections.Generic;
using AIDungeonPrompts.Backup.Persistence.Entities.Abstracts;

namespace AIDungeonPrompts.Backup.Persistence.Entities
{
	public class BackupPrompt : BackupBaseEntity
	{
		public string? AuthorsNote { get; set; }
		public string? Description { get; set; }
		public string? Memory { get; set; }
		public bool Nsfw { get; set; }
		public int? ParentId { get; set; }
		public string PromptContent { get; set; } = string.Empty;
		public DateTime? PublishDate { get; set; }
		public string? Quests { get; set; }
		public string Tags { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public byte[]? ScriptZip { get; set; }
		public string? NovelAiScenario { get; set; }
		public string? HoloAiScenario { get; set; }
		public List<BackupWorldInfo> WorldInfos { get; set; } = new List<BackupWorldInfo>();
	}
}
