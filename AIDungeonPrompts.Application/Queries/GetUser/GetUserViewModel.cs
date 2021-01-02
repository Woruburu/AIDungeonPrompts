using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Application.Queries.GetUser
{
	public class GetUserViewModel
	{
		public int Id { get; set; }
		public bool IsTransient { get; set; }
		public RoleEnum Role { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
