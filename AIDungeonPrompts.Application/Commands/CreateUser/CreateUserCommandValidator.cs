using System;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using FluentValidation;
using MediatR;

namespace AIDungeonPrompts.Application.Commands.CreateUser
{

	public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
	{
		public CreateUserCommandValidator()
		{
			RuleFor(e => e.Password).NotEmpty();
			RuleFor(e => e.Username).NotEmpty();
		}
	}
}
