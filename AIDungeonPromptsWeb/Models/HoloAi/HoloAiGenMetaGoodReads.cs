using System.Collections.Generic;

namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiGenMetaGoodReads
	{
		public string Author { get; set; } = string.Empty;
		public int PubDate { get; set; } = 2020;
		public List<HoloAiGenMetaTag> Tags { get; set; } = new();
		public int TargetLength { get; set; } = 25000;
	}
}
