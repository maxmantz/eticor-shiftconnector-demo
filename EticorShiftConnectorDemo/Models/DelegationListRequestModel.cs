namespace EticorShiftConnectorDemo.Models;

/// <summary>
/// Request model for querying delegations in the new API.
/// This model replaces DelegationsRequestModel and provides more filtering options.
/// </summary>
internal class DelegationListRequestModel : PageRequest
{
    /// <summary>
    /// If set, only delegations with the specified delegation ID will be returned.
    /// </summary>
    public int? DelegationId { get; set; }

    /// <summary>
    /// If set, only delegations with the specified task ID will be returned.
    /// </summary>
    public int? TaskId { get; set; }

    /// <summary>
    /// If set, only delegations with the specified responsible will be returned.
    /// </summary>
    public int? ResponsibleId { get; set; }

    /// <summary>
    /// If set, only delegations with the specified controller will be returned.
    /// </summary>
    public int? ControllerId { get; set; }

    /// <summary>
    /// If set, only delegations with the specified deputy will be returned.
    /// </summary>
    public int? DeputyId { get; set; }

    /// <summary>
    /// If set, only delegations with the specified employee will be returned.
    /// </summary>
    public int? EmployeeId { get; set; }

    /// <summary>
    /// Filter by start date.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter by end date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filter by organization unit path.
    /// </summary>
    public string? OrgUnitPath { get; set; }

    /// <summary>
    /// Filter by source path.
    /// </summary>
    public string? SourcePath { get; set; }

    /// <summary>
    /// If set, only delegations with the specified role ID will be returned.
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// If set, only delegations with the specified task package ID will be returned.
    /// </summary>
    public int? TaskPackageId { get; set; }

    /// <summary>
    /// Search query for filtering delegations.
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Filter by tag IDs.
    /// </summary>
    public string[]? TagIds { get; set; }

    /// <summary>
    /// Filter by whether delegations are used in task packages.
    /// </summary>
    public bool? UsedInTaskPackages { get; set; }

    /// <summary>
    /// Filter by risk factor.
    /// </summary>
    public int? RiskFactor { get; set; }

    /// <summary>
    /// Filter by delegation date type.
    /// </summary>
    public string? DelegationDateType { get; set; }

    /// <summary>
    /// The field to order results by (e.g., "duedate", "taskId").
    /// </summary>
    public string? OrderBy { get; set; } = "duedate";

    /// <summary>
    /// Whether to sort in descending order.
    /// </summary>
    public bool? Descending { get; set; }
}
