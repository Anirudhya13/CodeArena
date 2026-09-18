using AlgoJudge.Application.Common.Interfaces;
using AlgoJudge.Domain.ValueObjects;

namespace AlgoJudge.Application.AlgorithmProblems.Commands.UpdateAlgorithmProblem;

public record UpdateAlgorithmProblemCommand : IRequest
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public string? Colour { get; init; }
}

public class UpdateAlgorithmProblemCommandHandler : IRequestHandler<UpdateAlgorithmProblemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAlgorithmProblemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAlgorithmProblemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.AlgorithmProblems
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;

        if (request.Colour is not null)
        {
            entity.Colour = Colour.From(request.Colour);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
