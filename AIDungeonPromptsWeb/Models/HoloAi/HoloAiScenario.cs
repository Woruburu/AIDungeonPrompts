using System;
using System.Collections.Generic;
using System.Linq;
using AIDungeonPrompts.Application.Queries.GetPrompt;

namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiScenario
	{
		public HoloAiScenario()
		{

		}
		public HoloAiScenario(GetPromptViewModel prompt)
		{
			Title = prompt.Title;
			Content = prompt.PromptContent.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None).Select(e => new HoloAiContent(e)).ToList();
			Memory = prompt.Memory;
			AuthorsNote = prompt.AuthorsNote;
			WorldInfo = prompt.WorldInfos.Select(wi => new HoloAiWorldInfo(wi)).ToList();
			Tags = prompt.PromptTags.Select(e => e.Name).ToList();
		}

		public HoloAiGenMeta GenMeta { get; set; } = new();
		public List<string> Snippets => new();

		public int Version { get; set; } = 6;
		public string Title { get; set; }
		public List<HoloAiContent> Content { get; set; } = new();
		public string? Memory { get; set; }
		public string? AuthorsNote { get; set; }
		public List<HoloAiWorldInfo> WorldInfo { get; set; } = new();
		public List<string> Tags { get; set; } = new();
	}
}
