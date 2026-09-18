namespace CodeArena.Domain.Entities;

public class CodeSubmission : BaseAuditableEntity
{
    public int AlgorithmProblemId { get; set; }
    public AlgorithmProblem AlgorithmProblem { get; set; } = null!;

    public string? UserId { get; set; }
    
    public string? Language { get; set; }
    public string? Code { get; set; }
    
    public string? Verdict { get; set; }
    
    public string? TimeComplexity { get; set; }
    public string? SpaceComplexity { get; set; }
}

