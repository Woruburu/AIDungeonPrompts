using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Application.Queries.GetReports
{
	public class GetReportViewModel
	{
		public bool Cleared { get; set; }
		public string? ExtraDetails { get; set; }
		public int Id { get; set; }
		public int PromptId { get; set; }
		public string PromptTitle { get; set; } = string.Empty;
		public ReportReason ReportReason { get; set; }
	}
}
