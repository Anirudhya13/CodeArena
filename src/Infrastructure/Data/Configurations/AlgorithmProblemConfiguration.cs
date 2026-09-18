using CodeArena.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeArena.Infrastructure.Data.Configurations;

public class AlgorithmProblemConfiguration : IEntityTypeConfiguration<AlgorithmProblem>
{
    public void Configure(EntityTypeBuilder<AlgorithmProblem> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder
            .OwnsOne(b => b.Colour);
    }
}

