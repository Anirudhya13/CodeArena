namespace AlgoJudge.Application.Common.Interfaces;

public interface ISubmissionNotifier
{
    Task NotifyStatusAsync(string userId, string status);
}
