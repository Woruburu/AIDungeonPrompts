using System;
using System.Collections.Generic;
using System.Linq;

namespace AIDungeonPrompts.Application.Queries.GetPrompt
{
	public class GetPromptWorldInfoViewModel
	{
		public string Entry { get; set; } = string.Empty;
		public int Id { get; set; }
		public string Keys { get; set; } = string.Empty;

		public IEnumerable<string> KeysList => Keys
			.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
			.Select(e => e.Trim());
	}
}
