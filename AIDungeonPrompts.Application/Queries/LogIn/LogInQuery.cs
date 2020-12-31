using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Application.Queries.GetUser;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.LogIn
{
	public class LogInQuery : IRequest<GetUserViewModel>
	{
		public LogInQuery(string username, string password)
		{
			Username = username;
			Password = password;
		}

		public string Password { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
	}

	public class LogInQueryHandler : IRequestHandler<LogInQuery, GetUserViewModel>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public LogInQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<GetUserViewModel> Handle(LogInQuery request, CancellationToken cancellationToken = default)
		{
			var username = NpgsqlHelper.SafeIlike(request.Username);
			var user = await _dbContext
				.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(e => EF.Functions.ILike(e.Username, username, NpgsqlHelper.EscapeChar));

			if (user == null ||
				user.Password == null ||
				!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password))
			{
				throw new LoginFailedException();
			}

			return new GetUserViewModel()
			{
				Id = user.Id,
				Username = user.Username,
				Role = user.Role
			};
		}
	}
}
