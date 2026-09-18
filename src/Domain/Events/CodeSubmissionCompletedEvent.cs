namespace CodeArena.Domain.Events;

public class CodeSubmissionCompletedEvent : BaseEvent
{
    public CodeSubmissionCompletedEvent(CodeSubmission item)
    {
        Item = item;
    }

    public CodeSubmission Item { get; }
}

