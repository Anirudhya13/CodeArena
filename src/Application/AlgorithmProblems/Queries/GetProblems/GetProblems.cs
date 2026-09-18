using CodeArena.Application.Common.Interfaces;
using CodeArena.Application.Common.Models;
using CodeArena.Application.Common.Security;
using CodeArena.Domain.Enums;
using CodeArena.Domain.ValueObjects;

namespace CodeArena.Application.AlgorithmProblems.Queries.GetProblems;

[Authorize]
public record GetProblemsQuery : IRequest<ProblemsVm>;

public class GetProblemsQueryHandler : IRequestHandler<GetProblemsQuery, ProblemsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProblemsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProblemsVm> Handle(GetProblemsQuery request, CancellationToken cancellationToken)
    {
        return new ProblemsVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),

            Colours =
            [
                new ColourDto { Code = Colour.Grey, Name = nameof(Colour.Grey) },
                new ColourDto { Code = Colour.Purple, Name = nameof(Colour.Purple) },
                new ColourDto { Code = Colour.Blue, Name = nameof(Colour.Blue) },
                new ColourDto { Code = Colour.Teal, Name = nameof(Colour.Teal) },
                new ColourDto { Code = Colour.Green, Name = nameof(Colour.Green) },
                new ColourDto { Code = Colour.Orange, Name = nameof(Colour.Orange) },
                new ColourDto { Code = Colour.Red, Name = nameof(Colour.Red) },
            ],

            Lists = await _context.AlgorithmProblems
                .AsNoTracking()
                .ProjectTo<AlgorithmProblemDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };
    }
}

