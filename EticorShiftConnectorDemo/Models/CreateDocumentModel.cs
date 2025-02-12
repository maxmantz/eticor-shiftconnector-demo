namespace EticorShiftConnectorDemo.Models;

internal class CreateDocumentModel
{
    /// <summary>
    /// The name of the file
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// The content of the file. Maximum size is 5MB or 5242880 bytes.
    /// </summary>
    public byte[] Bytes { get; set; }

    /// <summary>
    /// The MIME type of the file
    /// </summary>
    public string MimeType { get; set; }
}
