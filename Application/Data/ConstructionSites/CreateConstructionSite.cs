using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.ConstructionSites
{
    public class CreateConstructionSite
    {
        public class Command : IRequest<Result<ConstructionSite>>
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public int ClientId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ConstructionSite>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreateConstructionSite> _logger;
            public Handler(ApplicationDbContext context, ILogger<CreateConstructionSite> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<ConstructionSite>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator<ConstructionSiteValidator.ConstructionSiteInput> constructionSiteValidator = new ConstructionSiteValidator();

                    try
                    {
                        constructionSiteValidator.Validate(new ConstructionSiteValidator.ConstructionSiteInput
                        {
                            Name = request.Name,
                            
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<ConstructionSite>.Failure(ex.ErrorsMessage);
                    }



                    var clientExist = _context.Clients.Any(x => x.Id == request.ClientId);

                    if (!clientExist)
                    {
                        _logger.LogWarning("Failed creating a contructionSite because there is no client was the one refered");
                        return Result<ConstructionSite>.Failure("Failed creating a contructionSite because there is no client was the one refered");
                    }

                    var constructionSiteExists = _context.ConstructionSites.Any(c => c.Name == request.Name);

                    if(constructionSiteExists)
                    {
                        _logger.LogWarning("Failed creating a constructionSite because is already created.");
                        return Result<ConstructionSite>.Failure("constructionSite is already  created.");
                    }

                    if(request.Latitude != null && request.Longitude != null)
                    {
                        var constructionSiteCoordsExists = _context.ConstructionSites.Any(c => c.Latitude == request.Latitude && 
                        c.Longitude == request.Longitude);

                        if (constructionSiteCoordsExists)
                        {
                            _logger.LogWarning("Failed creating a constructionSite because is already one with the same coords.");
                            return Result<ConstructionSite>.Failure("constructionSite is already created.");
                        }
                    }

                    request.Description ??= string.Empty;

                    var constructionSite = new ConstructionSite
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        ClientId = request.ClientId,
                        IsActive = true
                    };

                    await _context.ConstructionSites.AddAsync(constructionSite,cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to create constructionSite.");
                        return Result<ConstructionSite>.Failure("Failed to create constructionSite");
                    }

                    _logger.LogInformation("ConstructionSite created successfully.");
                    return Result<ConstructionSite>.Success(constructionSite);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating constructionSite");
                    return Result<ConstructionSite>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
