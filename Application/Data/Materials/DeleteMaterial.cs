using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.Materials
{
    public class DeleteMaterial
    {
        public class Command : IRequest<Result<string>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<DeleteMaterial> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<DeleteMaterial> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var material = await _context.Material
                        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
                    
                    if (material == null)
                    {
                        return Result<string>.Failure("Material not found.");
                    }

                    var name = material.Name;
                    _context.Material.Remove(material);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;


                    return result ? Result<string>.Success(name) : Result<string>.Failure("No changes on the context DB");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the material.");
                    return Result<string>.Failure("An error occurred while deleting the material.");
                }
            }
        }
    }
}
