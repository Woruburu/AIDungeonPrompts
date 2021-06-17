using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AIDungeonPrompts.Web.Models.NovelAi
{
	public class NovelAiLorebookEntry
	{
		[JsonPropertyName("text")]
		public string Text { get; set; } = string.Empty;

		[JsonPropertyName("keys")]
		public List<string> Keys { get; set; } = new();

		[JsonPropertyName("displayName")]
		public string DisplayName { get; set; } = string.Empty;
	}
}
