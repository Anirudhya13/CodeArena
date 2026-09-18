namespace AlgoJudge.Domain.Entities;

public class AlgorithmProblem : BaseAuditableEntity
{
    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.Grey;

    public string? Description { get; set; }

    public string? Difficulty { get; set; }

    public string? StarterCode { get; set; }

    public IList<CodeSubmission> Submissions { get; private set; } = new List<CodeSubmission>();
}


