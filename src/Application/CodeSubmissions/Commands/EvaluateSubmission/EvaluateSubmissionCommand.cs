using CodeArena.Application.Common.Interfaces;
using CodeArena.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CodeArena.Application.CodeSubmissions.Commands.EvaluateSubmission;

public record EvaluateSubmissionCommand(string Title, string Description, string Code, string Language, bool IsDraft = false) : IRequest<string>;

public class EvaluateSubmissionCommandHandler : IRequestHandler<EvaluateSubmissionCommand, string>
{
    private readonly IAiJudgeService _aiService;
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public EvaluateSubmissionCommandHandler(IAiJudgeService aiService, IApplicationDbContext context, IUser user)
    {
        _aiService = aiService;
        _context = context;
        _user = user;
    }

    public async Task<string> Handle(EvaluateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var problem = await _context.AlgorithmProblems.FirstOrDefaultAsync(p => p.Title == request.Title, cancellationToken);
        if (request.IsDraft && problem != null)
        {
            var draftSubmission = new CodeSubmission
            {
                AlgorithmProblemId = problem.Id,
                UserId = _user.Id ?? "Anonymous",
                Language = request.Language,
                Code = request.Code,
                Verdict = "Draft",
                TimeComplexity = "-",
                SpaceComplexity = "-"
            };
            _context.CodeSubmissions.Add(draftSubmission);
            await _context.SaveChangesAsync(cancellationToken);
            return "Saved as draft.";
        }        var response = await _aiService.EvaluateSubmissionAsync(request.Title, request.Description, request.Code, request.Language, cancellationToken);
        
        if (problem != null)
        {
            var isPass = Regex.IsMatch(response, @"(?i)verdict.*\bpass\b") || response.Contains("Pass (Accepted)");
            var verdictStr = isPass ? "Pass" : "Fail";

            var timeComplexityMatch = Regex.Match(response, @"(?i)Time Complexity.*?(O\([^\)]+\))");
            var spaceComplexityMatch = Regex.Match(response, @"(?i)Space Complexity.*?(O\([^\)]+\))");

            var submission = new CodeSubmission
            {
                AlgorithmProblemId = problem.Id,
                UserId = _user.Id ?? "Anonymous",
                Language = request.Language,
                Code = request.Code,
                Verdict = verdictStr,
                TimeComplexity = timeComplexityMatch.Success ? timeComplexityMatch.Groups[1].Value : null,
                SpaceComplexity = spaceComplexityMatch.Success ? spaceComplexityMatch.Groups[1].Value : null
            };

            _context.CodeSubmissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}






