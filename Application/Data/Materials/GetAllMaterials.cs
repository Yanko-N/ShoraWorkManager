using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Materials
{
    public class GetAllMaterials
    {
        public class Query : IRequest<Result<List<Material>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<Material>>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetAllMaterials> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetAllMaterials> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<List<Material>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var materials = await _context.Material
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    if (materials == null)
                    {
                        return Result<List<Material>>.Failure("Material not found.");
                    }

                    return Result<List<Material>>.Success(materials);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the material.");
                    return Result<List<Material>>.Failure("An error occurred while retrieving the material.");
                }
            }
        }
    }
}
