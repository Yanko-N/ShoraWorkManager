using MediatR;
using Application.Core;
using Application.Contracts.Response;
using Persistence.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Application.Data.WorkedHours
{
    public class GetWorkedHoursFromConstructionSite
    {
        public class Query : IRequest<Result<List<WorkedHoursDto>>>
        {
            public int ConstructionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<WorkedHoursDto>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetWorkedHoursFromConstructionSite> _logger;

            public Handler(ApplicationDbContext context, ILogger<GetWorkedHoursFromConstructionSite> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<List<WorkedHoursDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var workedHours = await _context.ContructionSiteWorkedHoursWorkers
                        .Include(x=>x.Worker)
                        .Where(x => x.ConstructionSiteId == request.ConstructionId)
                        .ToListAsync(cancellationToken);

                    Dictionary<Worker,List<(bool wasPaid,float time)>> result = new Dictionary<Worker,List<(bool,float)>>();

                    foreach (var worked in workedHours)
                    {
                        if(result.TryGetValue(worked.Worker, out _))
                        {
                            result[worked.Worker].Add((worked.WasPayed,worked.WorkedHours));
                        }
                        else
                        {
                            _ = result.TryAdd(worked.Worker, new List<(bool wasPaid, float time)>()
                            {
                                (worked.WasPayed,worked.WorkedHours)
                            });
                        }
                    }

                    List<WorkedHoursDto> returnedDto = result
                        .Select(x => new WorkedHoursDto() 
                        {
                            PaidHours = x.Value.Where(w => w.wasPaid).Sum(w => w.time),
                            UnpaidHours = x.Value.Where(w => !w.wasPaid).Sum(w=> w.time),
                            TotalHours = x.Value.Sum(w=> w.time),
                            WorkerId = x.Key.Id,
                            WorkerName = x.Key.Name
                        })
                        .ToList();

                    return Result<List<WorkedHoursDto>>.Success(returnedDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Some error occured when retriving the worked hours");
                    return Result<List<WorkedHoursDto>>.Failure("Some error occured when retriving the worked hours");
                }
            }
        }
    }
}
