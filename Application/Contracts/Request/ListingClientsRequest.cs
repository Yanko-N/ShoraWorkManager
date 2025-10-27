using Application.Enums;
using Application.Interfaces.Search;

namespace Application.Contracts.Request
{
    public class ListingClientsRequest : IRequestable, ISearchable,IOrderable, ISortable<ClientSortBy>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public ClientSortBy SortBy { get; set; } = ClientSortBy.None;
        public OrderByEnum OrderBy { get; set; } = OrderByEnum.Ascending;

    }
}
