using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.AuthorizationTokens
{
    public class GetLastAuthorizationToken
    {
        public class Query : IRequest<Result<Persistence.Models.AuthorizationToken>>
        {
        }
        public class Handler : IRequestHandler<Query, Result<Persistence.Models.AuthorizationToken>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<GetLastAuthorizationToken> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<GetLastAuthorizationToken> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Persistence.Models.AuthorizationToken>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var token = _context.AuthorizationTokens
                                        .Where(x => x.IsUsed == false)
                                        .AsNoTracking()
                                        .OrderByDescending(t => t.ExpirationDate)
                                        .FirstOrDefault();

                    if (token == null)
                    {
                        _logger.LogWarning("No authorization tokens found in the database.");
                        return Result<Persistence.Models.AuthorizationToken>.Failure("No authorization tokens found.");
                    }

                    if (token.ExpirationDate < DateTime.UtcNow)
                    {
                        _logger.LogWarning("The last authorization token has expired.");
                        return Result<Persistence.Models.AuthorizationToken>.Failure("The last authorization token has expired.");
                    }

                    _logger.LogInformation("The last authorization token was retrieved successfully.");
                    return Result<Persistence.Models.AuthorizationToken>.Success(token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the last authorization token.");
                    return Result<Persistence.Models.AuthorizationToken>.Failure($"An error occurred while retrieving the last authorization token");
                }                
            }
        }
    }
}
