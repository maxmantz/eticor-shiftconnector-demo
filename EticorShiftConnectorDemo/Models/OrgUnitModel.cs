namespace EticorShiftConnectorDemo.Models
{
    internal class OrgUnitModel
    {
        /// <summary>
        /// The ID of the organizational unit
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the organizational unit
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether the organizational unit is active
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// The ID of the parent organizational unit
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// The breadcrumb of the organizational unit
        /// Contains a pipe-separated string of parent IDs, e.g. "|1|2|3|"
        /// </summary>
        public string Breadcrumb { get; set; }
    }
}
