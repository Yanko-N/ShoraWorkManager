using Application.Core;
using Domain.Exceptions;
using Domain.Validators;
using Domain.Validators.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Workers
{
    public class CreateWorker
    {
        public class Command : IRequest<Result<Worker>>
        {
            public string Name { get; set; }
            public float PricePerHour { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Worker>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreateWorker> _logger;
            public Handler(ApplicationDbContext context, ILogger<CreateWorker> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Result<Worker>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    IDataValidator< WorkerValidator.WorkerInput> workerValidator = new WorkerValidator();

                    try
                    {
                        workerValidator.Validate(new WorkerValidator.WorkerInput
                        {
                            Name = request.Name,
                            PricePerHour = request.PricePerHour
                        });
                    }
                    catch (CustomValidationException ex)
                    {
                        return Result<Worker>.Failure(ex.ErrorsMessage);
                    }


                    var workerExists = _context.Workers.Any(c => c.Name == request.Name);

                    if(workerExists)
                    {
                        _logger.LogWarning("Failed creating a worker because is already in created.");
                        return Result<Worker>.Failure("worker is already in created.");
                    }

                    var worker = new Worker
                    {
                        Name = request.Name,
                        PricePerHour = request.PricePerHour
                    };

                    await _context.Workers.AddAsync(worker,cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        _logger.LogWarning("Failed to create worker.");
                        return Result<Worker>.Failure("Failed to create worker");
                    }

                    _logger.LogInformation("Worker created successfully.");
                    return Result<Worker>.Success(worker);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating worker");
                    return Result<Worker>.Failure("Something wrong occured, try again.");
                }
            }
        }
    }
}
