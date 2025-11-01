using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.ConstructionSites
{
    public class ChangeStateConstructionSite
    {
        public class Command : IRequest<Result<string>>
        {
            public int Id { get; set; }

            public bool JustFlipState { get; set; } = false;

            public bool NewState {  get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<ChangeStateConstructionSite> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<ChangeStateConstructionSite> logger)
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

                    constructionSite.IsActive = request.JustFlipState ? !constructionSite.IsActive : request.NewState;

                    _context.ConstructionSites.Update(constructionSite);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    var message = request.JustFlipState ? $"State was alternated with success of the construction site {name}" : $"State of Construction site {name} was successfully changed to {request.NewState}";

                    return result ? Result<string>.Success(message) : Result<string>.Failure("No changes on the context DB");
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
