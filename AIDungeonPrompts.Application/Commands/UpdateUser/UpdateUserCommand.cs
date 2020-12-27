using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Exceptions;
using AIDungeonPrompts.Application.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.UpdateUser
{
	public class UpdateUserCommand : IRequest
	{
		public int Id { get; set; }
		public string? Password { get; set; }
		public string? Username { get; set; }
	}

	public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public UpdateUserCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
		}

		public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
		{
			var user = await _dbContext.Users.FindAsync(request.Id);
			if (user == null)
			{
				throw new UserNotFoundException();
			}

			if (!string.IsNullOrWhiteSpace(request.Username))
			{
				if (await _dbContext
					.Users
					.FirstOrDefaultAsync(e =>
						EF.Functions.ILike(e.Username, NpgsqlHelper.SafeIlike(request.Username), NpgsqlHelper.EscapeChar) &&
						e.Id != request.Id) != null)
				{
					throw new UsernameNotUniqueException();
				}

				user.Username = request.Username;
			}

			if (!string.IsNullOrWhiteSpace(request.Password))
			{
				user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);
			}

			user.DateEdited = DateTime.UtcNow;

			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return Unit.Value;
		}
	}
}
