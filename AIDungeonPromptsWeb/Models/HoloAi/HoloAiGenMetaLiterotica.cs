using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiGenMetaLiterotica
	{
		public string Author { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public List<string> Tags { get; set; } = new();
		public int TargetLength { get; set; } = 5000;
	}
}
