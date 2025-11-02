using Application.Contracts.Response;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.MaterialMoviments
{
    public class GetMaterialFromConstructionSiteList
    {
        public class Query : IRequest<Result<List<MaterialMovement>>>
        {
            public int ConstructionSiteId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<MaterialMovement>>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetMaterialFromConstructionSiteList> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetMaterialFromConstructionSiteList> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<List<MaterialMovement>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var materialsMovement = await _context.MaterialMovements
                        .Include(x=> x.Material)
                        .Where(x => x.ConstructionSiteId == request.ConstructionSiteId)
                        .ToListAsync(cancellationToken);

                    materialsMovement ??= new List<MaterialMovement>(); 

                    return Result<List<MaterialMovement>>.Success(materialsMovement);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the material stock in the construction Site.");
                    return Result<List<MaterialMovement>>.Failure("An error occurred while retrieving the construction Site.");
                }
            }
        }
    }
}
