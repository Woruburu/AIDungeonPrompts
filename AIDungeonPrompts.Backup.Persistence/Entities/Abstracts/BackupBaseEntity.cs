using System;

namespace AIDungeonPrompts.Backup.Persistence.Entities.Abstracts
{
	public abstract class BackupBaseEntity
	{
		public int CorrelationId { get; set; }
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;
		public DateTime? DateEdited { get; set; }
		public int Id { get; set; }
	}
}
