using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Web.Models.User
{
	public class EditUserModel
	{
		public string? Password { get; set; }

		[Display(Name = "Please confirm your password")]
		public string? PasswordConfirm { get; set; }

		public string? Username { get; set; }
	}
}
