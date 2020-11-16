using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.SearchPrompts
{
	public enum TagJoin
	{
		[Display(Name = "Only show posts with ALL selected tags")]
		And,

		[Display(Name = "Show posts with ANY of the selected tags")]
		Or
	}
}
