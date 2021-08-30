namespace AIDungeonPrompts.Web.Models.HoloAi
{
	public class HoloAiContentChild
	{
		public HoloAiContentChild()
		{

		}
		public HoloAiContentChild(string text)
		{
			Text = text;
		}

		public string Text { get; set; } = string.Empty;
	}
}
