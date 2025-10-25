using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.AuthorizationTokens
{
    public class CreateAuthorizationToken
    {
        public class Query : IRequest<Result<Persistence.Models.AuthorizationToken>>
        {
        }
        public class Handler : IRequestHandler<Query, Result<Persistence.Models.AuthorizationToken>>
        {
            
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<CreateAuthorizationToken> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context,ILogger<CreateAuthorizationToken> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Persistence.Models.AuthorizationToken>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var tokens = await _context.AuthorizationTokens
                    .Where(x => x.IsUsed == false)
                    .OrderByDescending(t => t.ExpirationDate)
                    .ToListAsync(cancellationToken);

                    foreach (var token in tokens)
                    {
                        if (token.ExpirationDate < DateTime.UtcNow)
                        {
                            token.IsUsed = true;
                        }
                    }

                    var sucessfulSave = await _context.SaveChangesAsync(cancellationToken) >= 0;

                    if (!sucessfulSave)
                    {
                        return Result<Persistence.Models.AuthorizationToken>.Failure("Failed to update expired tokens.");
                    }

                    Persistence.Models.AuthorizationToken newToken = GenerateNewToken();

                    await _context.AuthorizationTokens.AddAsync(newToken);

                    sucessfulSave &= await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!sucessfulSave)
                    {
                        return Result<Persistence.Models.AuthorizationToken>.Failure("An error occurred when creating a new AuthorizationToken");
                    }

                    _logger.LogInformation("A new authorization token was created successfully.");
                    return Result<Persistence.Models.AuthorizationToken>.Success(newToken);
                }
                catch (Exception ex)
                {
                    return Result<Persistence.Models.AuthorizationToken>.Failure($"An error occurred while creating a new authorization token");
                }
            }

            Persistence.Models.AuthorizationToken GenerateNewToken()
            {
                var tokenValue = Guid.NewGuid().ToString("N"); 
                var expirationDate = DateTime.UtcNow.AddDays(1);

                return new Persistence.Models.AuthorizationToken
                {
                    Token = tokenValue,
                    ExpirationDate = expirationDate,
                    IsUsed = false
                };
            }
        }
    }
}
