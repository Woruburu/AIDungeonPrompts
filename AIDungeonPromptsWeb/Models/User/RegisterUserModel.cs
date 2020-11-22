namespace AIDungeonPrompts.Web.Models.User
{
	public class RegisterUserModel
	{
		public string Password { get; set; } = string.Empty;
		public string? ReturnUrl { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
