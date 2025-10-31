using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.ConstructionSites
{
    public class DesactivateConstructionSite
    {
        public class Command : IRequest<Result<string>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<DesactivateConstructionSite> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<DesactivateConstructionSite> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var constructionSite = await _context.ConstructionSites
                        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
                    
                    if (constructionSite == null)
                    {
                        return Result<string>.Failure("ConstructionSite not found.");
                    }

                    var name = constructionSite.Name;
                    constructionSite.IsActive = false;

                    _context.ConstructionSites.Update(constructionSite);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;


                    return result ? Result<string>.Success(name) : Result<string>.Failure("No changes on the context DB");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while desactivating the constructionSite.");
                    return Result<string>.Failure("An error occurred while desactivating the constructionSite.");
                }
            }
        }
    }
}
