using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Exceptions;
using AIDungeonPrompts.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.CreateUser
{
	public class CreateUserCommand : IRequest<int>
	{
		public string Password { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
	}

	public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public CreateUserCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
		{
			if (await _dbContext.Users.FirstOrDefaultAsync(e => e.Username == request.Username) != null)
			{
				throw new UsernameNotUniqueException();
			}

			var user = new User
			{
				DateCreated = DateTime.UtcNow,
				Password = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Username),
				Username = request.Password
			};

			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return user.Id;
		}
	}
}
