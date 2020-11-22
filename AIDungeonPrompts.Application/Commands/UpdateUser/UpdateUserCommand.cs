using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.UpdateUser
{
	public class UpdateUserCommand : IRequest
	{
		public int Id { get; set; }
		public string Password { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
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

			if (await _dbContext.Users.FirstOrDefaultAsync(e => e.Username == request.Username && e.Id != request.Id) != null)
			{
				throw new UsernameNotUniqueException();
			}

			user.Username = request.Username;
			user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);
			user.DateEdited = DateTime.UtcNow;

			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return Unit.Value;
		}
	}
}
