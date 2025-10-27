using Application.Enums;

namespace Application.Interfaces.Search
{
    public interface IRequestable
    {
        /// <summary>
        /// The current page number requested (starting from 1).
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// The number of items to return per page.
        /// </summary>
        int PageSize { get; set; }
    }


    /// <summary>
    /// Provides a basic search query string used to filter results.
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// The search keyword or term used for filtering.
        /// </summary>
        string Search { get; set; }
    }

    /// <summary>
    /// Defines a sort operation for a dataset based on a predefined enum.
    /// </summary>
    public interface IOrderable
    {
        /// <summary>
        /// The enum value indicating how results should be ordered (e.g., ascending, descending).
        /// </summary>
        OrderByEnum OrderBy { get; set; }
    }

    public interface ISortable<TSortBy> where TSortBy : Enum
    {
        /// <summary>
        /// The field or property to sort the results by, defined by the TSortBy enum.
        /// </summary>
        TSortBy SortBy { get; set; }
    }
}
