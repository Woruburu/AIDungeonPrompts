using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Domain.Enums
{
	public enum ReportReason
	{
		[Display(Name = "Illegal Content")]
		IllegalContent,

		[Display(Name = "Untagged NSFW")]
		UntaggedNsfw,

		[Display(Name = "Low Quality")]
		LowQuality,

		[Display(Name = "Needs Correction")]
		NeedsCorrection,

		[Display(Name = "Potential Duplicate")]
		Duplicate,

		[Display(Name = "Incorrect Tags")]
		IncorrectTags
	}
}
