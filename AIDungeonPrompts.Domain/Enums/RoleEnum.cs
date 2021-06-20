using System;
using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Domain.Enums
{
	[Flags]
	public enum RoleEnum
	{
		[Display(Name = "None")]
		None = 0,

		[Display(Name = "Tag Editor")]
		TagEdit = 1,

		[Display(Name = "Field Editor")]
		FieldEdit = 2,

		[Display(Name = "Delete")]
		Delete = 4
	}
}
