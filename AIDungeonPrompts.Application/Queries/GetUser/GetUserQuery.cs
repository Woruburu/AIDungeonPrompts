using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetUser
{
	public class GetUserQuery : IRequest<GetUserViewModel?>
	{
		public GetUserQuery(int id)
		{
			Id = id;
		}

		public int Id { get; set; }
	}

	public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserViewModel?>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public GetUserQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<GetUserViewModel?> Handle(GetUserQuery request, CancellationToken cancellationToken = default)
		{
			User? user = await _dbContext
				.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);

			if (user == null)
			{
				return null;
			}

			return new GetUserViewModel
			{
				Id = user.Id,
				Role = user.Role,
				Username = user.Username,
				IsTransient = string.IsNullOrWhiteSpace(user.Password)
			};
		}
	}
}
