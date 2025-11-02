using Application.Contracts.Response;
using Application.Core;
using Application.Enums;
using Application.Interfaces.Search;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.ConstructionSites
{
    public class GetConstructionSitesListing
    {
        public class Query : IRequestable, ISearchable, IOrderable, ISortable<ConstructionSiteSortBy>, IRequest<Result<PaginatedList<ConstructionSite>>>
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public string Search { get; set; }
            public ConstructionSiteSortBy SortBy { get; set; } = ConstructionSiteSortBy.None;
            public OrderByEnum OrderBy { get; set; } = OrderByEnum.Descending;
        }

        public class Handler : IRequestHandler<Query, Result<PaginatedList<ConstructionSite>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetConstructionSitesListing> _logger;
            public Handler(ApplicationDbContext applicationDbContext,ILogger<GetConstructionSitesListing> logger)
            {
                _context = applicationDbContext;
                _logger = logger;
            }

            public async Task<Result<PaginatedList<ConstructionSite>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!ValidateRequest(request))
                    {
                        return Result<PaginatedList<ConstructionSite>>.Failure("Invalid request parameters.");
                    }

                    var query = _context.ConstructionSites
                        .AsNoTracking()
                        .Include(c=>c.Owner)
                        .AsQueryable();

                    // search 
                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        var cleanedSearch = request.Search.ToLower().Trim();

                        query = query.Where(c =>
                            c.Name.ToLower().Trim().Contains(cleanedSearch) ||
                            c.Description.ToLower().Contains(cleanedSearch) ||
                            c.Owner.FirstName.ToLower().Contains(cleanedSearch) ||
                            c.Owner.LastName.ToLower().Contains(cleanedSearch) ||
                            c.Owner.Email.ToLower().Contains(cleanedSearch));
                    }

                    // sorting
                    query = request.SortBy switch
                    {
                        ConstructionSiteSortBy.Name => request.OrderBy == OrderByEnum.Descending ? 
                        query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),

                        ConstructionSiteSortBy.Description => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),

                        ConstructionSiteSortBy.OwnerName => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.Owner.FirstName) : query.OrderBy(c => c.Owner.FirstName),

                        ConstructionSiteSortBy.OwnerEmail => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.Owner.Email) : query.OrderBy(c => c.Owner.Email),

                        ConstructionSiteSortBy.State => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.IsActive) : query.OrderBy(c => c.IsActive),

                        _ => request.OrderBy == OrderByEnum.Descending ? //DEFAULT
                        query.OrderByDescending(c=>c.Id) : query.OrderBy(c=>c.Id),
                    };

                    // Apply pagination
                    var page = await PaginatedList<ConstructionSite>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

                    _logger.LogInformation("ConstructionSites listing retrieved successfully.");
                    return Result<PaginatedList<ConstructionSite>>.Success(page);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving constructionSites listing");
                    return Result<PaginatedList<ConstructionSite>>.Failure("An error occurred while retrieving constructionSites.");
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
