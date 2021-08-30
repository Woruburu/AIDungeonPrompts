using AIDungeonPrompts.Backup.Persistence.Entities.Abstracts;

namespace AIDungeonPrompts.Backup.Persistence.Entities
{
	public class BackupWorldInfo : BackupBaseEntity
	{
		public string Entry { get; set; } = string.Empty;
		public string Keys { get; set; } = string.Empty;
		public BackupPrompt? Prompt { get; set; }
		public int PromptId { get; set; }
	}
}
