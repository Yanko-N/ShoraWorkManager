using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.WorkedHours
{
    public class CreateWorkedHours
    {

        public class Command : IRequest<Result<ContructionSiteWorkedHoursWorker>>
        {
           
            public int ConstructionSiteId { get; set; }

            public int WorkerId { get; set; }

            public float WorkedHours { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<ContructionSiteWorkedHoursWorker>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<CreateWorkedHours> _logger;

            public Handler(ApplicationDbContext context, ILogger<CreateWorkedHours> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<ContructionSiteWorkedHoursWorker>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if(request.WorkedHours <= 0)
                    {
                        return Result<ContructionSiteWorkedHoursWorker>.Failure("The worked hours cannot be less or equal than 0");
                    }

                    var constructionSite = await _context.ConstructionSites
                        .FirstOrDefaultAsync(x => x.Id == request.ConstructionSiteId, cancellationToken);

                    if (constructionSite == null)
                    {
                        return Result<ContructionSiteWorkedHoursWorker>.Failure("Construction Site was not found");
                    }

                    var worker = await _context.Workers
                        .FirstOrDefaultAsync(x => x.Id == request.WorkerId, cancellationToken);

                    if(worker == null)
                    {
                        return Result<ContructionSiteWorkedHoursWorker>.Failure("Worker was not found");
                    }

                    var workedHours = new ContructionSiteWorkedHoursWorker()
                    {
                        ConstructionSiteId = request.ConstructionSiteId,
                        WorkerId = request.WorkerId,
                        WorkedHours = request.WorkedHours
                    };

                    await _context.ContructionSiteWorkedHoursWorkers.AddAsync(workedHours, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {

                        _logger.LogWarning("Failed to create Worked Hours.");
                        return Result<ContructionSiteWorkedHoursWorker>.Failure("Failed to create Worked Hours");
                    }

                    _logger.LogInformation("Worked Hours created successfully.");
                    return Result<ContructionSiteWorkedHoursWorker>.Success(workedHours);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating creating a worked hours");
                    return Result<ContructionSiteWorkedHoursWorker>.Failure("Error creating creating a worked hours");
                }
            }
        }
    }
}
