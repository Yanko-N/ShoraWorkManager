using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.ConstructionSites
{
    public class GetConstructionSite
    {
        public class Query : IRequest<Result<ConstructionSite>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ConstructionSite>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetConstructionSite> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetConstructionSite> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<ConstructionSite>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var constructionSite = await _context.ConstructionSites
                        .Include(c=>c.Owner)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == request.Id,cancellationToken);

                    if (constructionSite == null)
                    {
                        return Result<ConstructionSite>.Failure("ConstructionSite not found.");
                    }

                    return Result<ConstructionSite>.Success(constructionSite);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the constructionSite.");
                    return Result<ConstructionSite>.Failure("An error occurred while retrieving the constructionSite.");
                }
            }
        }
    }
}
