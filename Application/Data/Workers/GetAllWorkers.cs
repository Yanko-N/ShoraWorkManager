using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Workers
{
    public class GetAllWorkers
    {
        public class Query : IRequest<Result<List<Worker>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<Worker>>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetAllWorkers> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetAllWorkers> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<List<Worker>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var workers = await _context.Workers
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    if (workers == null)
                    {
                        return Result<List<Worker>>.Failure("Worker not found.");
                    }

                    return Result<List<Worker>>.Success(workers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the workers.");
                    return Result<List<Worker>>.Failure("An error occurred while retrieving the woerks.");
                }
            }
        }
    }
}
