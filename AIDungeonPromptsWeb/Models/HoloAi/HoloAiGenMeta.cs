namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiGenMeta
	{
		public int Dataset { get; set; } = 0;
		public HoloAiGenMetaLiterotica Literotica { get; set; } = new();
		public HoloAiGenMetaGoodReads GoodReads { get; set; } = new();
	}
}
