using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIDungeonPrompts.Application.Queries.GetPrompt
{

	public class GetPromptWorldInfoViewModel
	{
		public string Entry { get; set; } = string.Empty;
		public int Id { get; set; }
		public string Keys { get; set; } = string.Empty;
	}
}
