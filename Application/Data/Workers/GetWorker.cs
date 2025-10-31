using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Workers
{
    public class GetWorker
    {
        public class Query : IRequest<Result<Worker>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Worker>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetWorker> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetWorker> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Worker>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var worker = await _context.Workers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == request.Id,cancellationToken);

                    if (worker == null)
                    {
                        return Result<Worker>.Failure("Worker not found.");
                    }

                    return Result<Worker>.Success(worker);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the worker.");
                    return Result<Worker>.Failure("An error occurred while retrieving the worker.");
                }
            }
        }
    }
}
