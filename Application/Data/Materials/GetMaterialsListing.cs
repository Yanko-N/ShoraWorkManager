using Application.Contracts.Response;
using Application.Core;
using Application.Enums;
using Application.Interfaces.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Materials
{
    public class GetMaterialsListing
    {
        public class Query : IRequestable, ISearchable, IOrderable, ISortable<MaterialSortBy>, IRequest<Result<PaginatedList<Material>>>
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public string Search { get; set; }
            public MaterialSortBy SortBy { get; set; } = MaterialSortBy.None;
            public OrderByEnum OrderBy { get; set; } = OrderByEnum.Descending;
        }

        public class Handler : IRequestHandler<Query, Result<PaginatedList<Material>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetMaterialsListing> _logger;
            public Handler(ApplicationDbContext applicationDbContext,ILogger<GetMaterialsListing> logger)
            {
                _context = applicationDbContext;
                _logger = logger;
            }

            public async Task<Result<PaginatedList<Material>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!ValidateRequest(request))
                    {
                        return Result<PaginatedList<Material>>.Failure("Invalid request parameters.");
                    }

                    var query = _context.Material.AsQueryable();

                    // search 
                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        var cleanedSearch = request.Search.ToLower().Trim();

                        query = query.Where(c =>
                            c.Name.ToLower().Trim().Contains(cleanedSearch) ||
                            c.Description.ToLower().Trim().Contains(cleanedSearch));
                    }

                    // sorting
                    query = request.SortBy switch
                    {
                        MaterialSortBy.Name => request.OrderBy == OrderByEnum.Descending ? 
                        query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),

                        MaterialSortBy.Description => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),

                        MaterialSortBy.AvailableQuantity => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.AvailableQuantity) : query.OrderBy(c => c.AvailableQuantity),

                        _ => request.OrderBy == OrderByEnum.Descending ? //DEFAULT
                        query.OrderByDescending(c=>c.Id) : query.OrderBy(c=>c.Id),
                    };

                    // Apply pagination
                    var page = await PaginatedList<Material>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

                    _logger.LogInformation("Materials listing retrieved successfully.");
                    return Result<PaginatedList<Material>>.Success(page);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving materials listing");
                    return Result<PaginatedList<Material>>.Failure("An error occurred while retrieving materials.");
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
