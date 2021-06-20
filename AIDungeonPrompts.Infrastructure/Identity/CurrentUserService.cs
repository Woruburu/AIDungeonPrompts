using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Queries.GetUser;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Infrastructure.Identity
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly ILogger<CurrentUserService> _logger;
		private readonly IMediator _mediator;
		private GetUserViewModel? _currentUser;

		public CurrentUserService(ILogger<CurrentUserService> logger, IMediator mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		public async Task SetCurrentUser(int userId)
		{
			GetUserViewModel? user = await _mediator.Send(new GetUserQuery(userId));
			if (user == null)
			{
				_logger.LogWarning($"User with ID {userId} could not be found.");
			}

			_currentUser = user;
		}

		public bool TryGetCurrentUser(out GetUserViewModel? user)
		{
			user = _currentUser;
			return user != null;
		}
	}
}
