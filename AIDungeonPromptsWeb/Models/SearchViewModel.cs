using System.ComponentModel.DataAnnotations;
using AIDungeonPrompts.Application.Queries.SearchPrompts;

namespace AIDungeonPrompts.Web.Models
{
	public class SearchRequestParameters
	{
		[Display(Name = "Match Tags Exactly")]
		public bool MatchExact { get; set; } = true;

		public SearchNsfw NsfwSetting { get; set; }
		public int? Page { get; set; }
		public string? Query { get; set; }

		[Display(Name = "Reverse Results")]
		public bool Reverse { get; set; }

		[Display(Name = "Inclusive/Exclusive Tags")]
		public TagJoin TagJoin { get; set; }

		public string? Tags { get; set; }
	}

	public class SearchViewModel : SearchRequestParameters
	{
		public SearchPromptsViewModel SearchResult { get; set; } = new SearchPromptsViewModel();
	}
}
