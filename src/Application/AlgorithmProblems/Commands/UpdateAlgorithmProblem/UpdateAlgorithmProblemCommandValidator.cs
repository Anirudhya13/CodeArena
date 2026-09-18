using AlgoJudge.Application.Common.Interfaces;

namespace AlgoJudge.Application.AlgorithmProblems.Commands.UpdateAlgorithmProblem;

public class UpdateAlgorithmProblemCommandValidator : AbstractValidator<UpdateAlgorithmProblemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAlgorithmProblemCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(UpdateAlgorithmProblemCommand model, string title, CancellationToken cancellationToken)
    {
        return !await _context.AlgorithmProblems
            .Where(l => l.Id != model.Id)
            .AnyAsync(l => l.Title == title, cancellationToken);
    }
}
