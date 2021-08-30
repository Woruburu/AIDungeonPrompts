using System.Security.Claims;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Infrastructure.Identity
{
	public class CurrentUserMiddleware
	{
		private readonly ILogger<CurrentUserMiddleware> _logger;
		private readonly RequestDelegate _next;

		public CurrentUserMiddleware(RequestDelegate next, ILogger<CurrentUserMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
		{
			Claim? userIdClaim = context.User.FindFirst(e => e.Type == ClaimTypes.NameIdentifier);
			if (userIdClaim == null)
			{
				await _next(context);
				return;
			}

			if (!int.TryParse(userIdClaim.Value, out var id))
			{
				_logger.LogError($"Could not parse user id {userIdClaim.Value} as int");
				await _next(context);
				return;
			}

			await currentUserService.SetCurrentUser(id);
			await _next(context);
		}
	}
}
