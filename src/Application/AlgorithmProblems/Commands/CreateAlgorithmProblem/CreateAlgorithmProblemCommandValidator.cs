using AlgoJudge.Application.Common.Interfaces;

namespace AlgoJudge.Application.AlgorithmProblems.Commands.CreateAlgorithmProblem;

public class CreateAlgorithmProblemCommandValidator : AbstractValidator<CreateAlgorithmProblemCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAlgorithmProblemCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return !await _context.AlgorithmProblems
            .AnyAsync(l => l.Title == title, cancellationToken);
    }
}
