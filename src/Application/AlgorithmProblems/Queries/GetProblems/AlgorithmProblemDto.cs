using AlgoJudge.Domain.Entities;

namespace AlgoJudge.Application.AlgorithmProblems.Queries.GetProblems;

public class AlgorithmProblemDto
{
    public AlgorithmProblemDto()
    {
        Items = [];
    }

    public int Id { get; init; }

    public string? Title { get; init; }

    public string? Colour { get; init; }

    public string? Description { get; init; }

    public string? Difficulty { get; init; }

    public string? StarterCode { get; init; }

    public IReadOnlyCollection<CodeSubmissionDto> Items { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AlgorithmProblem, AlgorithmProblemDto>();
        }
    }
}

