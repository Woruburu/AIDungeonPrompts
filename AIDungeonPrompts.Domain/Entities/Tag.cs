using System.Collections.Generic;
using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class Tag : BaseDomainEntity
	{
		public string Name { get; set; } = string.Empty;
		public List<PromptTag> PromptTags { get; set; } = new List<PromptTag>();
	}
}
