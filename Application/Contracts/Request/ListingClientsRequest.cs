using Application.Enums;
using Application.Interfaces.Search;

namespace Application.Contracts.Request
{
    public class ListingClientsRequest : IRequestable, ISearchable, IOrderable, ISortable<ClientSortBy>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; } = string.Empty;
        public ClientSortBy SortBy { get; set; } = ClientSortBy.None;
        public OrderByEnum OrderBy { get; set; } = OrderByEnum.Ascending;

    }
}
