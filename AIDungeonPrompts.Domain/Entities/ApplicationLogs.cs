using System;

namespace AIDungeonPrompts.Domain.Entities
{
	public class ApplicationLog
	{
		public string? Exception { get; set; }
		public int Id { get; set; }
		public string Level { get; set; } = string.Empty;
		public string LogEvent { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string Properties { get; set; } = string.Empty;
		public string RenderedMessage { get; set; } = string.Empty;
		public DateTime TimeStamp { get; set; }
	}
}
