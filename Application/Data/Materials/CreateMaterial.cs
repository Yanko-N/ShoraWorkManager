using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Materials
{
    public class CreateMaterial
    {
        public class Command : IRequest<Result<Material>>
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public float AvailableQuantity { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Material>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreateMaterial> _logger;
            public Handler(ApplicationDbContext context, ILogger<CreateMaterial> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Material>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator< MaterialValidator.MaterialInput> materialValidator = new MaterialValidator();

                    try
                    {
                        materialValidator.Validate(new MaterialValidator.MaterialInput
                        {
                            Name = request.Name,
                            Description = request.Description,
                            AvailableQuantity = request.AvailableQuantity
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<Material>.Failure(ex.ErrorsMessage);
                    }

                    //If is null set to empty
                    request.Description ??= string.Empty;

                    var materialExists = _context.Material.Any(c => c.Name == request.Name);

                    if(materialExists)
                    {
                        _logger.LogWarning("Failed creating a material because is already in created.");
                        return Result<Material>.Failure("material is already in created.");
                    }

                    var material = new Material
                    {
                        Name = request.Name,
                        Description = request.Description,
                        AvailableQuantity = request.AvailableQuantity
                    };

                    await _context.Material.AddAsync(material,cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to create material.");
                        return Result<Material>.Failure("Failed to create material");
                    }

                    _logger.LogInformation("Material created successfully.");
                    return Result<Material>.Success(material);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating material");
                    return Result<Material>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
