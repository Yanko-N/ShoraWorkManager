using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Materials
{
    public class GetMaterial
    {
        public class Query : IRequest<Result<Material>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Material>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetMaterial> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetMaterial> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Material>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var material = await _context.Material
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == request.Id,cancellationToken);

                    if (material == null)
                    {
                        return Result<Material>.Failure("Material not found.");
                    }

                    return Result<Material>.Success(material);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the material.");
                    return Result<Material>.Failure("An error occurred while retrieving the material.");
                }
            }
        }
    }
}
