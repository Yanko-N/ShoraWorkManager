using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Payments
{
    public class GetPaymentsByContructionAndWorker
    {
        public class Query : IRequest<Result<List<Payment>>>
        {
            public int ConstructionId { get; set; }
            public int WorkerId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Payment>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetPaymentsByContructionAndWorker> _logger;
            public Handler(ApplicationDbContext context, ILogger<GetPaymentsByContructionAndWorker> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<List<Payment>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<Payment> toReturn = new List<Payment>();

                    var list = await _context.ContructionSiteWorkedHoursWorkers
                        .Where(x => x.ConstructionSiteId == request.ConstructionId && x.WorkerId == request.WorkerId && x.WasPayed)
                        .ToListAsync(cancellationToken);

                    foreach (var item in list) 
                    {
                        var payment = await _context.Payments.FirstOrDefaultAsync(x => x.ContructionSiteWorkedHoursWorkerId == item.Id, cancellationToken);

                        if (payment == null)
                        {
                            continue;
                        }

                        toReturn.Add(payment);
                    }

                    _logger.LogInformation("Payments were gotten successfully.");
                    return Result<List<Payment>>.Success(toReturn);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Some error occured when getting the payment");
                    return Result<List<Payment>>.Failure("Some error occured when getting the payment");
                }
            }
        }
    }
}
