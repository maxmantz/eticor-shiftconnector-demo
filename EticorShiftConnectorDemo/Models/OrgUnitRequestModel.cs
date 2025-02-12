namespace EticorShiftConnectorDemo.Models;

internal class OrgUnitRequestModel : PageRequest
{
    /// <summary>
    /// If set, only returns org units delegated to this region.
    /// </summary>
    public int? RegionId { get; set; }

    /// <summary>
    /// If set, only returns org units where this employee is delegated.
    /// </summary>
    public int? EmployeeId { get; set; }
    /// <summary>
    /// If set, only returns sites if <value>true</value> or no sites if <value>false</value>.
    /// </summary>
    public bool? IsSite { get; set; }

    /// <summary>
    /// If set, only returns org units that have this value.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// If set, only returns OrgUnits that have this parent ID.
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// If set, only returns OrgUnits with write access
    /// </summary>
    public bool? IsWriteable { get; set; }
}
