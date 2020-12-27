using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.LogIn
{
	public class LogInQuery : IRequest<LogInQueryViewModel>
	{
		public string Password { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
	}

	public class LogInQueryHandler : IRequestHandler<LogInQuery, LogInQueryViewModel>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public LogInQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<LogInQueryViewModel> Handle(LogInQuery request, CancellationToken cancellationToken)
		{
			var user = await _dbContext
				.Users
				.FirstOrDefaultAsync(e => EF.Functions.ILike(e.Username, NpgsqlHelper.SafeIlike(request.Username), NpgsqlHelper.EscapeChar));
			if (user == null)
			{
				throw new LoginFailedException();
			}
			if (user.Password == null)
			{
				throw new LoginFailedException();
			}

			if (!BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password))
			{
				throw new LoginFailedException();
			}

			return new LogInQueryViewModel()
			{
				Id = user.Id,
				Username = user.Username
			};
		}
	}
}
