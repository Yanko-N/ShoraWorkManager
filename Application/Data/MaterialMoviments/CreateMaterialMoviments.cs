using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.MaterialMoviments
{
    public class CreateMaterialMoviments
    {
        public class Command : IRequest<Result<MaterialMovement>>
        {
            public int MaterialId { get; set; }
            public int ConstructionId { get; set; }
            public float Quantity { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<MaterialMovement>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreateMaterialMoviments> _logger;
            public Handler(ApplicationDbContext context, ILogger<CreateMaterialMoviments> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<MaterialMovement>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    //With tracking because we are going to remove the used stock
                    var material = await _context.Material
                        .FirstOrDefaultAsync(x => x.Id == request.MaterialId, cancellationToken);

                    if (material == null)
                    {
                        return Result<MaterialMovement>.Failure("Material doesnt exist");
                    }

                    var constructionSiteExist = await _context.ConstructionSites
                        .AnyAsync(x => x.Id == request.ConstructionId, cancellationToken);

                    if (!constructionSiteExist)
                    {
                        return Result<MaterialMovement>.Failure("Construction Site doesnt exist");
                    }


                    if(request.Quantity == 0)
                    {
                        return Result<MaterialMovement>.Failure("Cannot move a value of 0");
                    }

                    if(request.Quantity > 0 && material.AvailableQuantity <= request.Quantity)
                    {
                        return Result<MaterialMovement>.Failure("Cannot move to the site more stock than the avaiable one");
                    }

                    if(request.Quantity < 0)
                    {
                        var alreadyUsedQuantity = await _context.MaterialMovements
                            .Where(x => x.ConstructionSiteId == request.ConstructionId)
                            .SumAsync(x => x.Quantity);

                        if(alreadyUsedQuantity <= 0 || Math.Abs(request.Quantity) > Math.Abs(alreadyUsedQuantity))
                        {
                            return Result<MaterialMovement>.Failure("Cannot move from the site more stock than avaiable there");
                        }
                    }

                    var newAvaiableQuantity = material.AvailableQuantity + request.Quantity;

                    material.AvailableQuantity = newAvaiableQuantity;
                    _context.Material.Update(material);

                    var materialMovement = new MaterialMovement
                    {
                        ConstructionSiteId = request.ConstructionId,
                        MaterialId = request.MaterialId,
                        Quantity = request.Quantity,
                        MovementDate = DateTime.Now
                    };

                    await _context.MaterialMovements.AddAsync(materialMovement, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to create MaterialMovement.");
                        return Result<MaterialMovement>.Failure("Failed to create MaterialMovement");
                    }

                    _logger.LogInformation("MaterialMovement created successfully.");
                    return Result<MaterialMovement>.Success(materialMovement);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating MaterialMovement");
                    return Result<MaterialMovement>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
