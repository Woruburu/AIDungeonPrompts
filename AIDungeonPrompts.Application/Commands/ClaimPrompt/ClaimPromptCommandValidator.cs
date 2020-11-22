using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using FluentValidation;
using MediatR;

namespace AIDungeonPrompts.Application.Commands.ClaimPrompt
{

	public class ClaimPromptCommandValidator : AbstractValidator<ClaimPromptCommand>
	{
		public ClaimPromptCommandValidator()
		{
			RuleFor(e => e.OwnerId).NotEmpty();
			RuleFor(e => e.PromptId).NotEmpty();
		}
	}
}
