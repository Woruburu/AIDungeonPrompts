using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.CreateTransientUser
{
	public class CreateTransientUserCommand : IRequest<int>
	{
	}

	public class CreateTransientUserCommandHandler : IRequestHandler<CreateTransientUserCommand, int>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public CreateTransientUserCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<int> Handle(CreateTransientUserCommand request, CancellationToken cancellationToken)
		{
			var username = Guid.NewGuid().ToString();
			while (!await UsernameIsUnique(username))
			{
				username = Guid.NewGuid().ToString();
			}

			var user = new User
			{
				Username = username,
				DateCreated = DateTime.UtcNow
			};

			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return user.Id;
		}

		private async Task<bool> UsernameIsUnique(string username)
		{
			return (await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username)) == null;
		}
	}
}
