using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetScript
{

	public class GetScriptQueryValidator : AbstractValidator<GetScriptQuery>
	{
		public GetScriptQueryValidator()
		{
			RuleFor(e => e.PromptId).NotEmpty();
		}
	}
}
