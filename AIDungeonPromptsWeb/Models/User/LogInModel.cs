using AIDungeonPrompts.Application.Queries.LogIn;

namespace AIDungeonPrompts.Web.Models.User
{
	public class LogInModel
	{
		public LogInQuery LogInQuery { get; set; } = new LogInQuery();
		public string? ReturnUrl { get; set; }
	}
}
