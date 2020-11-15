using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class ReportConfiguration : IEntityTypeConfiguration<Report>
	{
		public void Configure(EntityTypeBuilder<Report> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.ReportReason).IsRequired();
			builder.HasOne(e => e.Prompt)
				.WithMany(e => e.Reports)
				.HasForeignKey(e => e.PromptId)
				.IsRequired();
		}
	}
}
