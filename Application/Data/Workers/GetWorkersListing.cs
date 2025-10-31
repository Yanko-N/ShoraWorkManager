using Application.Contracts.Response;
using Application.Core;
using Application.Enums;
using Application.Interfaces.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Workers
{
    public class GetWorkersListing
    {
        public class Query : IRequestable, ISearchable, IOrderable, ISortable<WorkerSortBy>, IRequest<Result<PaginatedList<Worker>>>
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public string Search { get; set; }
            public WorkerSortBy SortBy { get; set; } = WorkerSortBy.None;
            public OrderByEnum OrderBy { get; set; } = OrderByEnum.Descending;
        }

        public class Handler : IRequestHandler<Query, Result<PaginatedList<Worker>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetWorkersListing> _logger;
            public Handler(ApplicationDbContext applicationDbContext,ILogger<GetWorkersListing> logger)
            {
                _context = applicationDbContext;
                _logger = logger;
            }

            public async Task<Result<PaginatedList<Worker>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!ValidateRequest(request))
                    {
                        return Result<PaginatedList<Worker>>.Failure("Invalid request parameters.");
                    }

                    var query = _context.Workers.AsQueryable();

                    // search 
                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        var cleanedSearch = request.Search.ToLower().Trim();

                        query = query.Where(c =>
                            c.Name.ToLower().Trim().Contains(cleanedSearch));
                    }

                    // sorting
                    query = request.SortBy switch
                    {
                        WorkerSortBy.Name => request.OrderBy == OrderByEnum.Descending ? 
                        query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),

                        WorkerSortBy.PricePerHour => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.PricePerHour) : query.OrderBy(c => c.PricePerHour),

                        _ => request.OrderBy == OrderByEnum.Descending ? //DEFAULT
                        query.OrderByDescending(c=>c.Id) : query.OrderBy(c=>c.Id),
                    };

                    // Apply pagination
                    var page = await PaginatedList<Worker>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

                    _logger.LogInformation("Workers listing retrieved successfully.");
                    return Result<PaginatedList<Worker>>.Success(page);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving workers listing");
                    return Result<PaginatedList<Worker>>.Failure("An error occurred while retrieving workers.");
                }
            }

            private bool ValidateRequest(Query request)
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
