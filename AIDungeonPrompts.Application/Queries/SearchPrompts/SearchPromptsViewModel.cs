using System.Collections.Generic;

namespace AIDungeonPrompts.Application.Queries.SearchPrompts
{
	public class SearchPromptsViewModel
	{
		public List<SearchPromptsResultViewModel> Results { get; set; } = new List<SearchPromptsResultViewModel>();
		public int TotalPages { get; set; }
	}
}
