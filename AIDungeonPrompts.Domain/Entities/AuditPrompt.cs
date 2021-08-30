using System;
using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class AuditPrompt : BaseDomainEntity
	{
		public Guid AuditScopeId { get; set; }
		public string Entry { get; set; } = string.Empty;
		public int PromptId { get; set; }
	}
}
