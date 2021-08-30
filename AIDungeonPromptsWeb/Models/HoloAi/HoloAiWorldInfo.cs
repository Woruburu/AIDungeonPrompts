using System.Collections.Generic;
using System.Linq;
using AIDungeonPrompts.Application.Queries.GetPrompt;

namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiWorldInfo
	{
		public HoloAiWorldInfo()
		{

		}
		public HoloAiWorldInfo(GetPromptWorldInfoViewModel worldInfo)
		{
			Keys = worldInfo.KeysList.ToList();
			Value = worldInfo.Entry;
		}

		public List<string> Keys { get; set; } = new();
		public string Value { get; set; } = string.Empty;
		public int Rank => 1;
	}
}
