using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.DeletePrompt
{
	public class DeletePromptCommand : IRequest
	{
		public DeletePromptCommand(int id)
		{
			Id = id;
		}

		public int Id { get; set; }
	}

	public class DeletePromptCommandHandler : IRequestHandler<DeletePromptCommand>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public DeletePromptCommandHandler(IAIDungeonPromptsDbContext dbContext, ICurrentUserService currentUserService)
		{
			_dbContext = dbContext;
			_currentUserService = currentUserService;
		}

		public async Task<Unit> Handle(DeletePromptCommand request, CancellationToken cancellationToken = default)
		{
			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				throw new DeletePromptUserUnauthorizedException();
			}

			var prompt = await _dbContext.Prompts.Include(e => e.Children).FirstOrDefaultAsync(e => e.Id == request.Id);

			if (prompt == null)
			{
				throw new DeletePromptDoesNotExistException();
			}

			if (prompt.OwnerId != user!.Id && (user.Role & RoleEnum.Delete) == 0)
			{
				throw new DeletePromptUserUnauthorizedException();
			}

			_dbContext.Prompts.Remove(prompt);
			await RemoveAllChildren(prompt.Children);

			await _dbContext.SaveChangesAsync(cancellationToken);

			return Unit.Value;
		}

		private async Task RemoveAllChildren(List<Prompt> children)
		{
			foreach (var child in children)
			{
				await _dbContext.Entry(child).Collection(e => e.Children).LoadAsync();
				await RemoveAllChildren(child.Children);
				_dbContext.Prompts.Remove(child);
			}
		}
	}
}
