using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.ConstructionSites
{
    public class EditConstructionSite
    {
        public class Command : IRequest<Result<ConstructionSite>>
        {

            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public int ClientId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ConstructionSite>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<EditConstructionSite> _logger;
            public Handler(ApplicationDbContext context, ILogger<EditConstructionSite> logger)
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
                            Description = request.Description,
                            Latitude = request.Latitude,
                            Longitude = request.Longitude,
                            ClientId = request.ClientId
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<ConstructionSite>.Failure(ex.ErrorsMessage);
                    }


                    var constructionSiteWithName = await _context.ConstructionSites
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Name == request.Name,cancellationToken);

                    if (constructionSiteWithName != null && constructionSiteWithName.Id != request.Id)
                    {
                        _logger.LogWarning("Failed editing a ConstructionSite because Name is already in use.");
                        return Result<ConstructionSite>.Failure("Name is already in use.");
                    }

                    if (request.Latitude != null && request.Longitude != null)
                    {
                        var constructionSiteCoordsExists = await _context.ConstructionSites.FirstOrDefaultAsync(c => c.Latitude == request.Latitude &&
                        c.Longitude == request.Longitude,cancellationToken);

                        if (constructionSiteCoordsExists != null && constructionSiteCoordsExists.Id != request.Id)
                        {
                            _logger.LogWarning("Failed creating a constructionSite because is already one with the same coords.");
                            return Result<ConstructionSite>.Failure("constructionSite is already created.");
                        }
                    }

                    var constructionSiteToUpdate = await _context.ConstructionSites
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                    if (constructionSiteToUpdate == null)
                    {
                        _logger.LogWarning("ConstructionSite not found.");
                        return Result<ConstructionSite>.Failure("ConstructionSite not found.");
                    }


                    if (!constructionSiteToUpdate.IsActive)
                    {
                        _logger.LogInformation($"The Inactive Construction Site {constructionSiteToUpdate.Id} was unsuccefuly alterated, because is inactive");
                        return Result<ConstructionSite>.Failure("An Inactive Construction Site cannot be alterated");
                    }

                    request.Description ??= string.Empty;

                    constructionSiteToUpdate.Name = request.Name;
                    constructionSiteToUpdate.Description = request.Description;
                    constructionSiteToUpdate.Latitude = request.Latitude;
                    constructionSiteToUpdate.Longitude = request.Longitude;
                    constructionSiteToUpdate.ClientId = request.ClientId;

                    _context.ConstructionSites.Update(constructionSiteToUpdate);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to edit ConstructionSite.");
                        return Result<ConstructionSite>.Failure("Failed to edit ConstructionSite");
                    }

                    _logger.LogInformation("ConstructionSite edited successfully.");
                    return Result<ConstructionSite>.Success(constructionSiteToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("Concurrency error editing ConstructionSite");
                    return Result<ConstructionSite>.Failure("Concurrency error occured, try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing ConstructionSite");
                    return Result<ConstructionSite>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
