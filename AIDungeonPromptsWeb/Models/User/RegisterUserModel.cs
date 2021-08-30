using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Web.Models.User
{
	public class RegisterUserModel
	{
		public string Password { get; set; } = string.Empty;

		[Display(Name = "Please confirm your password")]
		public string PasswordConfirm { get; set; } = string.Empty;

		public string? ReturnUrl { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
