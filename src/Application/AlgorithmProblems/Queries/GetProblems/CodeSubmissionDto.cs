using CodeArena.Domain.Entities;

namespace CodeArena.Application.AlgorithmProblems.Queries.GetProblems;

public class CodeSubmissionDto
{
    public int Id { get; init; }
    public string Language { get; init; } = null!;
    public string Verdict { get; init; } = null!;
    
    private class Mapping : AutoMapper.Profile
    {
        public Mapping()
        {
            CreateMap<CodeSubmission, CodeSubmissionDto>();
        }
    }
}

