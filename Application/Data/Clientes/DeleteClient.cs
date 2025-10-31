using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.Clientes
{
    public class DeleteClient
    {
        public class Command : IRequest<Result<string>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<DeleteClient> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<DeleteClient> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var client = await _context.Clients
                        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
                    
                    if (client == null)
                    {
                        return Result<string>.Failure("Client not found.");
                    }

                    var name = client.FirstName + " " + client.LastName;
                    _context.Clients.Remove(client);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;


                    return result ? Result<string>.Success(name) : Result<string>.Failure("No changes on the context DB");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the client.");
                    return Result<string>.Failure("An error occurred while deleting the client.");
                }
            }
        }
    }
}
