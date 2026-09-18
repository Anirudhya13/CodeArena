using CodeArena.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeArena.Application.CodeSubmissions.Queries.GetUserSubmissions;

public class SubmissionDto
{
    public int Id { get; set; }
    public string ProblemTitle { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Verdict { get; set; } = null!;
    public string? TimeComplexity { get; set; }
    public string? SpaceComplexity { get; set; }
    public DateTimeOffset Created { get; set; }
}

public record GetUserSubmissionsQuery : IRequest<List<SubmissionDto>>;

public class GetUserSubmissionsQueryHandler : IRequestHandler<GetUserSubmissionsQuery, List<SubmissionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetUserSubmissionsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<List<SubmissionDto>> Handle(GetUserSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id ?? "Anonymous";
        return await _context.CodeSubmissions
            .Include(s => s.AlgorithmProblem)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Id)
            .Select(s => new SubmissionDto
            {
                Id = s.Id,
                ProblemTitle = s.AlgorithmProblem.Title ?? "Unknown",
                Language = s.Language,
                Code = s.Code,
                Verdict = s.Verdict,
                TimeComplexity = s.TimeComplexity,
                SpaceComplexity = s.SpaceComplexity,
                Created = s.Created
            })
            .ToListAsync(cancellationToken);
    }
}


