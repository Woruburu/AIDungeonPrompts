namespace AIDungeonPrompts.Application.Queries.GetAllTags
{
	public class GetTagViewModel
	{
		public int Count { get; internal set; }
		public int Id { get; internal set; }
		public string Name { get; internal set; } = string.Empty;
	}
}
