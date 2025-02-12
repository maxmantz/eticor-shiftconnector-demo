namespace EticorShiftConnectorDemo.Models;

internal class PageRequest
{
    /// <summary>
    /// The offset of the page (number of items to skip)
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// The limit of the page (number of items per page)
    /// </summary>
    public int Limit { get; set; } = 10;
}
