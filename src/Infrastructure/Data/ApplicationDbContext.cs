using System.Reflection;
using CodeArena.Application.Common.Interfaces;
using CodeArena.Domain.Entities;
using CodeArena.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeArena.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<AlgorithmProblem> AlgorithmProblems => Set<AlgorithmProblem>();

    public DbSet<CodeSubmission> CodeSubmissions => Set<CodeSubmission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

