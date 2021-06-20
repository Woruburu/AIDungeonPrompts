using AIDungeonPrompts.Domain.Entities.Abstract;

namespace AIDungeonPrompts.Domain.Entities
{
	public class ServerFlag : BaseDomainEntity
	{
		public string Name { get; set; } = string.Empty;
		public bool Enabled { get; set; }
		public string? AdditionalMessage { get; set; }
	}

	public static class ServerFlagName
	{
		public static readonly string CreateDisabled = "CREATE_DISABLED";
	}
}
