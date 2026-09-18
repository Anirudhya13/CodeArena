using CodeArena.Application.Common.Interfaces;

namespace CodeArena.Application.AlgorithmProblems.Commands.DeleteAlgorithmProblem;

public record DeleteAlgorithmProblemCommand(int Id) : IRequest;

public class DeleteAlgorithmProblemCommandHandler : IRequestHandler<DeleteAlgorithmProblemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAlgorithmProblemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAlgorithmProblemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.AlgorithmProblems
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.AlgorithmProblems.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

