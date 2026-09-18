using CodeArena.Application.Common.Interfaces;

namespace CodeArena.Application.CodeSubmissions.Commands.DeleteCodeSubmission;

public record DeleteCodeSubmissionCommand(int Id) : IRequest;

public class DeleteCodeSubmissionCommandHandler : IRequestHandler<DeleteCodeSubmissionCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCodeSubmissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCodeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CodeSubmissions
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.CodeSubmissions.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

}

