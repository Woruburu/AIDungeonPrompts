using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using FluentValidation;
using MediatR;

namespace AIDungeonPrompts.Application.Commands.CreateReport
{

	public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
	{
		public CreateReportCommandValidator()
		{
			RuleFor(e => e.PromptId).NotEmpty();
		}
	}
}
