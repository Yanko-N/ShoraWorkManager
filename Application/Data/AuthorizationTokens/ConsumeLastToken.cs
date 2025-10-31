using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Data.AuthorizationTokens
{
    public class ConsumeLastToken
    {
        public class Command : IRequest<Result<Persistence.Models.AuthorizationToken>>
        {
            public string Token { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Persistence.Models.AuthorizationToken>>
        {
            private readonly Persistence.Data.ApplicationDbContext _context;
            private readonly ILogger<ConsumeLastToken> _logger;
            public Handler(Persistence.Data.ApplicationDbContext context, ILogger<ConsumeLastToken> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Persistence.Models.AuthorizationToken>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if(string.IsNullOrWhiteSpace(request.Token))
                    {
                        return Result<Persistence.Models.AuthorizationToken>.Failure("The provided token is invalid.");
                    }

                    var token = await _context.AuthorizationTokens
                        .FirstOrDefaultAsync(x=>x.Token == request.Token,cancellationToken);

                    if(token == null)
                    {
                        _logger.LogWarning("The provided token was not found.");
                        return Result<Persistence.Models.AuthorizationToken>.Failure("The provided token was not found.");
                    }

                    if (token.IsUsed)
                    {
                        _logger.LogWarning("The provided token has already been used.");
                        return Result<Persistence.Models.AuthorizationToken>.Failure("The provided token has already been used.");
                    }

                    token.IsUsed = true;

                    _ = _context.AuthorizationTokens.Update(token);

                    var sucessfulSave = await _context.SaveChangesAsync(cancellationToken) >= 0;

                    if (!sucessfulSave)
                    {
                        _logger.LogWarning("Failed to update expired tokens.");
                        return Result<Persistence.Models.AuthorizationToken>.Failure("Failed to update expired tokens.");
                    }

                    _logger.LogInformation("A  authorization token was consumed successfully.");
                    return Result<Persistence.Models.AuthorizationToken>.Success(token);
                }
                catch (Exception ex)
                {
                    return Result<Persistence.Models.AuthorizationToken>.Failure($"An error occurred while consuming the last authorization token");
                }
            }
        }
    }
}
