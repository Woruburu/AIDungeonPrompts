using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.SearchPrompts
{
	public enum TagJoin
	{
		[Display(Name = "Results must include all tags")]
		And,

		[Display(Name = "Results can include any tag")]
		Or,

		[Display(Name = "Results exclude all tags")]
		None
	}
}
