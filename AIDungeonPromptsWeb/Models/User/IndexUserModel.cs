using AIDungeonPrompts.Application.Queries.SearchPrompts;
using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Web.Models.User
{
	public class IndexUserModel
	{
		public bool IsTransient { get; set; }
		public int? Page { get; set; }
		public string Username { get; set; } = string.Empty;
		public SearchPromptsViewModel UserPrompts { get; set; } = new SearchPromptsViewModel();
		public RoleEnum UserRoles { get; set; }
	}
}
