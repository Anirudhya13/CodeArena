using AlgoJudge.Application.Common.Interfaces;
using AlgoJudge.Domain.Entities;
using AlgoJudge.Domain.ValueObjects;

namespace AlgoJudge.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;

public record CreateAlgorithmProblemCommand : IRequest<int>
{
    public string? Title { get; init; }

    public string? Colour { get; init; }
}

public class CreateAlgorithmProblemCommandHandler : IRequestHandler<CreateAlgorithmProblemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateAlgorithmProblemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAlgorithmProblemCommand request, CancellationToken cancellationToken)
    {
        var entity = new AlgorithmProblem
        {
            Title = request.Title,
            Colour = Colour.From(request.Colour ?? Colour.Grey)
        };

        _context.AlgorithmProblems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
