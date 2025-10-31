using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Clientes
{
    public class GetAllClients
    {
        public class Query : IRequest<Result<List<Client>>>
        {

        }

        public class Handler : IRequestHandler<Query, Result<List<Client>>>
        {

            public readonly ILogger<GetAllClients> _logger;
            public readonly ApplicationDbContext _context;

            public Handler(ILogger<GetAllClients> logger, ApplicationDbContext context)
            {
                _logger = logger;
                _context = context;
            }

            public async Task<Result<List<Client>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Client>? clients = await _context.Clients.ToListAsync(cancellationToken);

                    if (clients == null)
                    {
                        _logger.LogWarning("Clients get all List was null");
                        return Result<List<Client>>.Success(new List<Client>());
                    }

                    return Result<List<Client>>.Success(clients);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Some error occured when getting all Clients List");
                    return Result<List<Client>>.Failure("Some error occured when getting all Clients List");
                }
            }
        }
    }
}
