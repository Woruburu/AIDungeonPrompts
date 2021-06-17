using System.Text.Json.Serialization;

namespace AIDungeonPrompts.Web.Models.NovelAi
{
	public class NovelAiContextConfig
	{
		[JsonPropertyName("prefix")]
		public string Prefix { get; set; } = string.Empty;

		[JsonPropertyName("suffix")]
		public string Suffix { get; set; } = string.Empty;

		[JsonPropertyName("tokenBudget")]
		public int TokenBudget { get; set; }

		[JsonPropertyName("reservedTokens")]
		public int ReservedTokens { get; set; }

		[JsonPropertyName("budgetPriority")]
		public int BudgetPriority { get; set; }

		[JsonPropertyName("trimDirection")]
		public string TrimDirection { get; set; } = string.Empty;

		[JsonPropertyName("insertionType")]
		public string InsertionType { get; set; } = string.Empty;

		[JsonPropertyName("insertionPosition")]
		public int InsertionPosition { get; set; }
	}
}
