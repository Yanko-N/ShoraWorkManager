using Application.Enums;
using Application.Interfaces.Search;

namespace Application.Contracts.Request
{
    public class ListingConstructionSiteRequest : IRequestable, ISearchable, IOrderable, ISortable<ConstructionSiteSortBy>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; } = string.Empty;
        public ConstructionSiteSortBy SortBy { get; set; } = ConstructionSiteSortBy.None;
        public OrderByEnum OrderBy { get; set; } = OrderByEnum.Ascending;

    }
}
