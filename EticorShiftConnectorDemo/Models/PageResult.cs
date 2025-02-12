namespace EticorShiftConnectorDemo.Models;

internal class PageResult<T> where T : class
{
    /// <summary>
    /// The number of items in the result.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The request's offset.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Total number of results. Used to calculate pagination.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// The items.
    /// </summary>
    public IList<T> Items { get; set; } = [];
}
