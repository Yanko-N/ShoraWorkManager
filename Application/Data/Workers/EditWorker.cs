using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Workers
{
    public class EditWorker
    {
        public class Command : IRequest<Result<Worker>>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public float PricerPerHour { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Worker>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<EditWorker> _logger;
            public Handler(ApplicationDbContext context, ILogger<EditWorker> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Worker>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator<WorkerValidator.WorkerInput> workerValidator = new WorkerValidator();

                    try
                    {
                        workerValidator.Validate(new WorkerValidator.WorkerInput
                        {
                            Name = request.Name,
                            PricePerHour = request.PricerPerHour
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<Worker>.Failure(ex.ErrorsMessage);
                    }

                    var workerWithName = await _context.Workers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Name == request.Name,cancellationToken);

                    if (workerWithName != null && workerWithName.Id != request.Id)
                    {
                        _logger.LogWarning("Failed editing a Worker because Name is already in use.");
                        return Result<Worker>.Failure("Name is already in use.");
                    }

                    var workerToUpdate = await _context.Workers
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                    if (workerToUpdate == null)
                    {
                        _logger.LogWarning("Worker not found.");
                        return Result<Worker>.Failure("Worker not found.");
                    }

                    workerToUpdate.Name = request.Name;
                    workerToUpdate.PricePerHour = request.PricerPerHour;

                    _context.Workers.Update(workerToUpdate);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to edit Worker.");
                        return Result<Worker>.Failure("Failed to edit Worker");
                    }

                    _logger.LogInformation("Worker edited successfully.");
                    return Result<Worker>.Success(workerToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError("Concurrency error editing Worker");
                    return Result<Worker>.Failure("Concurrency error occured, try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing Worker");
                    return Result<Worker>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
