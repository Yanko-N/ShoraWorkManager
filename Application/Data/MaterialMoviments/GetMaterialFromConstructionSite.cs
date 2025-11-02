using Application.Contracts.Response;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.MaterialMoviments
{
    public class GetMaterialFromConstructionSite
    {
        public class Query : IRequest<Result<List<MaterialStockDto>>>
        {
            public int ConstructionSiteId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<MaterialStockDto>>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetMaterialFromConstructionSite> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetMaterialFromConstructionSite> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<List<MaterialStockDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var materialsMovement = await _context.MaterialMovements
                        .Include(x=> x.Material)
                        .Where(x => x.ConstructionSiteId == request.ConstructionSiteId)
                        .ToListAsync(cancellationToken);


                    Dictionary<Material,float> materialsAndStock = new Dictionary<Material, float>();
                    foreach (MaterialMovement materialMov in materialsMovement)
                    {

                        if(materialsAndStock.TryGetValue(materialMov.Material, out float value))
                        {
                            materialsAndStock[materialMov.Material] = value + materialMov.Quantity;
                        }
                        else
                        {
                            _ = materialsAndStock.TryAdd(materialMov.Material, materialMov.Quantity);
                        }
                    }

                    List<MaterialStockDto> returnedDto = materialsAndStock.Select(x => new MaterialStockDto()
                    {
                        MaterialId = x.Key.Id,
                        MaterialName = x.Key.Name,
                        Quantity = x.Value
                    }).ToList();

                    return Result<List<MaterialStockDto>>.Success(returnedDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the material stock in the construction Site.");
                    return Result<List<MaterialStockDto>>.Failure("An error occurred while retrieving the construction Site.");
                }
            }
        }
    }
}
