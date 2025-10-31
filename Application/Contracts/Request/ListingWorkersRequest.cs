using Application.Enums;
using Application.Interfaces.Search;

namespace Application.Contracts.Request
{
    public class ListingWorkersRequest : IRequestable, ISearchable, IOrderable, ISortable<WorkerSortBy>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; } = string.Empty;
        public WorkerSortBy SortBy { get; set; } = WorkerSortBy.None;
        public OrderByEnum OrderBy { get; set; } = OrderByEnum.Ascending;

    }
}
