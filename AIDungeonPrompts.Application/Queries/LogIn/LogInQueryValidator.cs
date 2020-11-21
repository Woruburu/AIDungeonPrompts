using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using FluentValidation;
using MediatR;

namespace AIDungeonPrompts.Application.Queries.LogIn
{

	public class LogInQueryValidator : AbstractValidator<LogInQuery>
	{
		public LogInQueryValidator()
		{
			RuleFor(e => e.Password).NotEmpty();
			RuleFor(e => e.Username).NotEmpty();
		}
	}
}
