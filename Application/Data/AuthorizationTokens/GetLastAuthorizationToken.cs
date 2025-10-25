using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            public Handler(Persistence.Data.ApplicationDbContext context)
            {
                _context = context;
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
                        return Result<Persistence.Models.AuthorizationToken>.Failure("No authorization tokens found.");
                    }

                    if (token.ExpirationDate < DateTime.UtcNow)
                    {
                        return Result<Persistence.Models.AuthorizationToken>.Failure("The last authorization token has expired.");
                    }

                    return Result<Persistence.Models.AuthorizationToken>.Success(token);
                }
                catch (Exception ex)
                {
                    return Result<Persistence.Models.AuthorizationToken>.Failure($"An error occurred while retrieving the last authorization token");
                }                
            }
        }
    }
}
