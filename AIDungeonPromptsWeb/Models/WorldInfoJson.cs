using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Commands.CreateTransientUser;
using AIDungeonPrompts.Application.Commands.DeletePrompt;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using AIDungeonPrompts.Domain.Enums;
using AIDungeonPrompts.Web.Extensions;
using AIDungeonPrompts.Web.Models.Prompts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.Controllers
{

	public class WorldInfoJson
	{
		public string Entry { get; set; } = string.Empty;
		public string Keys { get; set; } = string.Empty;
	}
}
