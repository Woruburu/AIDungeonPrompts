using System;

namespace AIDungeonPrompts.Domain.Entities.Abstract
{
	public abstract class BaseDomainEntity
	{
		public DateTime DateCreated { get; set; } = DateTime.UtcNow;
		public DateTime? DateEdited { get; set; }
		public int Id { get; set; }
	}
}
