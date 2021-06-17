using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AIDungeonPrompts.Application.Queries.GetPrompt;

namespace AIDungeonPrompts.Web.Models.NovelAi
{
	public class NovelAiScenario
	{
		public NovelAiScenario(GetPromptViewModel prompt)
		{
			Title = prompt.Title;
			Description = prompt.Description ?? string.Empty;
			Prompt = prompt.PromptContent;
			Tags = prompt.PromptTags.Select(e => e.Name).ToList();
			Context = new List<NovelAiContext>
			{
				new()
				{
					Text = prompt.Memory?.Trim() ?? string.Empty,
					Config = new NovelAiContextConfig
					{
						Prefix = string.Empty,
						Suffix = "\n",
						TokenBudget = 2048,
						ReservedTokens = 0,
						BudgetPriority = 800,
						TrimDirection = "trimBottom",
						InsertionType = "token",
						InsertionPosition = 0
					}
				}, new()
				{
					Text = prompt.AuthorsNote?.Trim() ?? string.Empty,
					Config = new NovelAiContextConfig
					{
						Prefix = string.Empty,
						Suffix = "\n",
						TokenBudget = 2048,
						ReservedTokens = 2048,
						BudgetPriority = -400,
						TrimDirection = "trimBottom",
						InsertionType = "newline",
						InsertionPosition = -4
					}
				}
			};
			Lorebook = new NovelAiLorebook
			{
				LorebookEntries =
					prompt.WorldInfos.Select(
						wi =>
						{
							var split = wi.Keys.Split(",");
							return new NovelAiLorebookEntry
							{
								Keys = split.Select(key => key.Trim()).ToList(),
								Text = wi.Entry,
								DisplayName = split.FirstOrDefault()?.Trim() ?? string.Empty
							};
						}).ToList()
			};
		}

		[JsonPropertyName("scenarioVersion")]
		public int ScenarioVersion => 0;

		[JsonPropertyName("title")]
		public string Title { get; set; } = string.Empty;

		[JsonPropertyName("description")]
		public string Description { get; set; } = string.Empty;

		[JsonPropertyName("prompt")]
		public string Prompt { get; set; } = string.Empty;

		[JsonPropertyName("tags")]
		public List<string> Tags { get; set; } = new();

		[JsonPropertyName("context")]
		public List<NovelAiContext> Context { get; set; } = new();

		[JsonPropertyName("lorebook")]
		public NovelAiLorebook Lorebook { get; set; } = new();
	}
}
