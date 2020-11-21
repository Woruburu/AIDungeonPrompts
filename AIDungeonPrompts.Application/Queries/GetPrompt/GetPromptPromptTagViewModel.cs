using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.GetPrompt
{
	public class GetPromptPromptTagViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}
}
