using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Web.Models.User
{
	public class LogInModel
	{
		[Required]
		public string Password { get; set; } = string.Empty;

		public string? ReturnUrl { get; set; }

		[Required]
		public string Username { get; set; } = string.Empty;
	}
}
