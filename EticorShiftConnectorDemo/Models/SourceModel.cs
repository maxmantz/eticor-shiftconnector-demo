namespace EticorShiftConnectorDemo.Models;

internal class SourceModel
{
    /// <summary>
    /// The ID of the source.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The source's paragraph.
    /// </summary>
    public string Paragraph { get; set; }

    /// <summary>
    /// The source's law.
    /// </summary>
    public LawModel Law { get; set; }
}
