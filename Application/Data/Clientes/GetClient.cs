using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;

namespace Application.Data.Clientes
{
    public class GetClient
    {
        public class Query : IRequest<Result<Client>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Client>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetClient> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetClient> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Client>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var client = await _context.Clients
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == request.Id,cancellationToken);

                    if (client == null)
                    {
                        return Result<Client>.Failure("Client not found.");
                    }

                    return Result<Client>.Success(client);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the client.");
                    return Result<Client>.Failure("An error occurred while retrieving the client.");
                }
            }
        }
    }
}
