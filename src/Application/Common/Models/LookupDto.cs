using AlgoJudge.Domain.Entities;

namespace AlgoJudge.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AlgorithmProblem, LookupDto>();
            CreateMap<CodeSubmission, LookupDto>();
        }
    }
}
