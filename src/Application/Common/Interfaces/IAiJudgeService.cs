namespace AlgoJudge.Application.Common.Interfaces;

public interface IAiJudgeService
{
    Task<string> EvaluateSubmissionAsync(string problemTitle, string problemDescription, string userCode, string language, CancellationToken cancellationToken = default);
}
