using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class WorldInfo : BaseDomainEntity
	{
		public string Entry { get; set; } = string.Empty;
		public string Keys { get; set; } = string.Empty;
		public Prompt? Prompt { get; set; }
		public int PromptId { get; set; }
	}
}
