using System.Collections.Generic;
using AIDungeonPrompts.Domain.Entities.Abstract;
using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Domain.Entities
{
	public class User : BaseDomainEntity
	{
		public string? Password { get; set; }
		public List<Prompt> Prompts { get; set; } = new List<Prompt>();
		public RoleEnum Role { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
