using CodeArena.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeArena.Infrastructure.Data.Configurations;

public class CodeSubmissionConfiguration : IEntityTypeConfiguration<CodeSubmission>
{
    public void Configure(EntityTypeBuilder<CodeSubmission> builder)
    {
        builder.HasOne(s => s.AlgorithmProblem)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.AlgorithmProblemId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(s => s.UserId).HasMaxLength(450);
    }
}

