using CodeArena.Domain.Events;
using Microsoft.Extensions.Logging;

namespace CodeArena.Application.CodeSubmissions.EventHandlers;

public class LogCodeSubmissionCompleted : INotificationHandler<CodeSubmissionCompletedEvent>
{
    private readonly ILogger<LogCodeSubmissionCompleted> _logger;

    public LogCodeSubmissionCompleted(ILogger<LogCodeSubmissionCompleted> logger)
    {
        _logger = logger;
    }

    public Task Handle(CodeSubmissionCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CodeArena Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}

