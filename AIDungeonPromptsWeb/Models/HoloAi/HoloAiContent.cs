using System;
using System.Collections.Generic;
using System.Linq;

namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiContent
	{
		public HoloAiContent()
		{

		}
		public HoloAiContent(string content)
		{
			Children = new List<HoloAiContentChild> { new(content) };
		}

		public string Type => "paragraph";
		public List<HoloAiContentChild> Children { get; set; } = new();
	}
}
