using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Web.ColorScheme
{
	public enum ColorSchemePreference
	{
		[Display(Name = "Light Scheme")]
		Light = 0,

		[Display(Name = "Dark Scheme")]
		Dark = 1
	}
}
