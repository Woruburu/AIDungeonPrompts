using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class PromptTag : BaseDomainEntity
	{
		public Prompt? Prompt { get; set; }
		public int PromptId { get; set; }
		public Tag? Tag { get; set; }
		public int TagId { get; set; }
	}
}
