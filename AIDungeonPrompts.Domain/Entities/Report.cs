using AIDungeonPrompts.Domain.Entities.Abstract;
using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Domain.Entities
{
	public class Report : BaseDomainEntity
	{
		public string? ExtraDetails { get; set; }
		public Prompt? Prompt { get; set; }
		public int PromptId { get; set; }
		public ReportReason ReportReason { get; set; }
	}
}
