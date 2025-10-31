using Application.Enums;
using Application.Interfaces.Search;

namespace Application.Contracts.Request
{
    public class ListingMaterialsRequest : IRequestable, ISearchable, IOrderable, ISortable<MaterialSortBy>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; } = string.Empty;
        public MaterialSortBy SortBy { get; set; } = MaterialSortBy.None;
        public OrderByEnum OrderBy { get; set; } = OrderByEnum.Ascending;

    }
}
