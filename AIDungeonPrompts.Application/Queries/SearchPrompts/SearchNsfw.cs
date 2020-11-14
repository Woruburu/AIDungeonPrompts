using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.SearchPrompts
{
	public enum SearchNsfw
	{
		[Display(Name = "SFW & NSFW")]
		Both,

		[Display(Name = "SFW only")]
		SafeOnly,

		[Display(Name = "NSFW only")]
		NsfwOnly
	}
}
