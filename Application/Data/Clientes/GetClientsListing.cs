using Application.Contracts.Response;
using Application.Core;
using Application.Enums;
using Application.Interfaces.Search;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;
using Persistence.Models;

namespace Application.Data.Clientes
{
    public class GetClientsListing
    {
        public class Query : IRequestable, ISearchable, IOrderable, ISortable<ClientSortBy>, IRequest<Result<PaginatedList<Client>>>
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public string Search { get; set; }
            public ClientSortBy SortBy { get; set; } = ClientSortBy.None;
            public OrderByEnum OrderBy { get; set; } = OrderByEnum.Descending;
        }

        public class Handler : IRequestHandler<Query, Result<PaginatedList<Client>>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetClientsListing> _logger;
            public Handler(ApplicationDbContext applicationDbContext,ILogger<GetClientsListing> logger)
            {
                _context = applicationDbContext;
                _logger = logger;
            }

            public async Task<Result<PaginatedList<Client>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!ValidateRequest(request))
                    {
                        return Result<PaginatedList<Client>>.Failure("Invalid request parameters.");
                    }

                    var query = _context.Clients.AsQueryable();

                    // search 
                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        var cleanedSearch = request.Search.ToLower().Trim();

                        query = query.Where(c =>
                            c.FirstName.ToLower().Trim().Contains(cleanedSearch) ||
                            c.LastName.ToLower().Trim().Contains(cleanedSearch) ||
                            c.Email.ToLower().Trim().Contains(cleanedSearch) ||
                            c.PhoneNumber.ToLower().Trim().Contains(cleanedSearch));
                    }

                    // sorting
                    query = request.SortBy switch
                    {
                        ClientSortBy.FirstName => request.OrderBy == OrderByEnum.Descending ? 
                        query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),

                        ClientSortBy.LastName => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),

                        ClientSortBy.Email => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),

                        ClientSortBy.PhoneNumber => request.OrderBy == OrderByEnum.Descending ?
                        query.OrderByDescending(c => c.PhoneNumber) : query.OrderBy(c => c.PhoneNumber),

                        _ => request.OrderBy == OrderByEnum.Descending ? //DEFAULT
                        query.OrderByDescending(c=>c.Id) : query.OrderBy(c=>c.Id),
                    };

                    // Apply pagination
                    var page = await PaginatedList<Client>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

                    return Result<PaginatedList<Client>>.Success(page);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving clients listing");
                    return Result<PaginatedList<Client>>.Failure("An error occurred while retrieving clients.");
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
