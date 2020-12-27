using AIDungeonPrompts.Application.Queries.SearchPrompts;

namespace AIDungeonPrompts.Web.Models.User
{
	public class IndexUserModel
	{
		public int? Page { get; set; }
		public string Username { get; set; } = string.Empty;
		public SearchPromptsViewModel UserPrompts { get; set; } = new SearchPromptsViewModel();
	}
}
