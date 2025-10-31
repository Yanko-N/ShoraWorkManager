using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Materials
{
    public class EditMaterial
    {
        public class Command : IRequest<Result<Material>>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public float AvailableQuantity { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Material>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<EditMaterial> _logger;
            public Handler(ApplicationDbContext context, ILogger<EditMaterial> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Material>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator<MaterialValidator.MaterialInput> materialValidator = new MaterialValidator();

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

                    var materialWithName = await _context.Material
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Name == request.Name,cancellationToken);

                    if (materialWithName != null && materialWithName.Id != request.Id)
                    {
                        _logger.LogWarning("Failed editing a Material because Name is already in use.");
                        return Result<Material>.Failure("Name is already in use.");
                    }

                    var materialToUpdate = await _context.Material
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                    if (materialToUpdate == null)
                    {
                        _logger.LogWarning("Material not found.");
                        return Result<Material>.Failure("Material not found.");
                    }

                    materialToUpdate.Name = request.Name;
                    materialToUpdate.Description = request.Description;
                    materialToUpdate.AvailableQuantity = request.AvailableQuantity;

                    _context.Material.Update(materialToUpdate);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to edit Material.");
                        return Result<Material>.Failure("Failed to edit Material");
                    }

                    _logger.LogInformation("Material edited successfully.");
                    return Result<Material>.Success(materialToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("Concurrency error editing Material");
                    return Result<Material>.Failure("Concurrency error occured, try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing Material");
                    return Result<Material>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
