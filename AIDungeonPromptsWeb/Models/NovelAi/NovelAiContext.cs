using System.Text.Json.Serialization;

namespace AIDungeonPrompts.Web.Models.NovelAi
{
	public class NovelAiContext
	{
		[JsonPropertyName("text")]
		public string Text { get; set; } = string.Empty;

		[JsonPropertyName("contextConfig")]
		public NovelAiContextConfig Config { get; set; } = new();
	}
}
