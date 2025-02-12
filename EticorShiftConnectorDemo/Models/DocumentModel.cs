namespace EticorShiftConnectorDemo.Models
{
    internal class DocumentModel
    {
        /// <summary>
        /// The ID of the document
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The file name of the document
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The content of the document. Only included if getting a document by ID.
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}
