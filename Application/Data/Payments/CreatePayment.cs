using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Payments
{
    public class CreatePayment
    {
        public class Command : IRequest<Result<string>>
        {
            public int ConstructionId { get; set; }
            public int WorkerId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreatePayment> _logger;
            public Handler(ApplicationDbContext context,ILogger<CreatePayment> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var constructionSite = await _context.ConstructionSites.FirstOrDefaultAsync(x => x.Id == request.ConstructionId, cancellationToken);

                    if (constructionSite == null) {
                        return Result<string>.Failure("Construction Site was not found");
                    }

                    var worker = await _context.Workers.FirstOrDefaultAsync(x => x.Id == request.WorkerId, cancellationToken);

                    if (worker == null)
                    {
                        return Result<string>.Failure("Worker was not found");
                    }

                    var unpaidHours = await _context.ContructionSiteWorkedHoursWorkers
                        .Where(x=> x.WorkerId == request.WorkerId && x.ConstructionSiteId == request.ConstructionId && !x.WasPayed)
                        .ToListAsync(cancellationToken);

                    if (!unpaidHours.Any())
                    {
                        return Result<string>.Failure("No hours to pay");
                    }

                    List<Payment> toAdd = new List<Payment>();

                    foreach(var unpaidHour in unpaidHours)
                    {
                        var payment = new Payment()
                        {
                            ContructionSiteWorkedHoursWorkerId = unpaidHour.Id,
                            Value = unpaidHour.WorkedHours * worker.PricePerHour
                        };

                        unpaidHour.WasPayed = true;
                        toAdd.Add(payment);
                    }


                    await _context.Payments.AddRangeAsync(toAdd,cancellationToken);
                    _context.ContructionSiteWorkedHoursWorkers.UpdateRange(unpaidHours);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {

                        _logger.LogWarning("Failed to create payments.");
                        return Result<string>.Failure("Failed to create payments");
                    }

                    _logger.LogInformation("Payments created successfully.");
                    return Result<string>.Success("Payments were created successufully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Some error occured when creating the payment");
                    return Result<string>.Failure("Some error occured when creating the payment");
                }
            }
        }
    }
}
