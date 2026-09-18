using AlgoJudge.Domain.Entities;

namespace AlgoJudge.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AlgorithmProblem> AlgorithmProblems { get; }

    DbSet<CodeSubmission> CodeSubmissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
