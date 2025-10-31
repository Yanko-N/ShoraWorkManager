using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.Workers
{
    public class DeleteWorker
    {
        public class Command : IRequest<Result<string>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<DeleteWorker> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<DeleteWorker> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var worker = await _context.Workers
                        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
                    
                    if (worker == null)
                    {
                        return Result<string>.Failure("Worker not found.");
                    }

                    var name = worker.Name;
                    _context.Workers.Remove(worker);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;


                    return result ? Result<string>.Success(name) : Result<string>.Failure("No changes on the context DB");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the worker.");
                    return Result<string>.Failure("An error occurred while deleting the worker.");
                }
            }
        }
    }
}
