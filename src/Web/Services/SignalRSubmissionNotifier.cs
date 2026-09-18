using AlgoJudge.Application.Common.Interfaces;
using AlgoJudge.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AlgoJudge.Web.Services;

public class SignalRSubmissionNotifier : ISubmissionNotifier
{
    private readonly IHubContext<SubmissionHub> _hubContext;

    public SignalRSubmissionNotifier(IHubContext<SubmissionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyStatusAsync(string userId, string status)
    {
        return _hubContext.Clients.User(userId).SendAsync("ReceiveStatus", status);
    }
}
