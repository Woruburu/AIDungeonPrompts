using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AIDungeonPrompts.Web.Models.NovelAi
{
	public class NovelAiLorebook
	{
		[JsonPropertyName("lorebookVersion")]
		public int LorebookVersion => 1;

		[JsonPropertyName("entries")]
		public List<NovelAiLorebookEntry> LorebookEntries { get; set; } = new();
	}
}
