using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Infrastructure.Identity
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;
		private readonly ILogger<CurrentUserService> _logger;
		private User? _currentUser;

		public CurrentUserService(IAIDungeonPromptsDbContext dbContext, ILogger<CurrentUserService> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		public async Task SetCurrentUser(int userId)
		{
			var user = await _dbContext.Users.FindAsync(userId);
			if (user == null)
			{
				_logger.LogWarning($"User with ID {userId} could not be found.");
			}
			_currentUser = user;
		}

		public bool TryGetCurrentUser(out User? user)
		{
			user = _currentUser;
			return user != null;
		}
	}
}
