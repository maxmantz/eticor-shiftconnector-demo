namespace EticorShiftConnectorDemo.Models;

internal class DelegationModel
{
    /// <summary>
    /// The ID of the delegation
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the responsible employee
    /// </summary>
    public int ResponsibleId { get; set; }

    /// <summary>
    /// The responsible employee. Only included in the response if extend contains "employees".
    /// </summary>
    public EmployeeModel Responsible { get; set; }

    /// <summary>
    /// The ID of the org unit
    /// </summary>
    public int? OrgUnitId { get; set; }

    /// <summary>
    /// The org unit. Only included in the response if extend contains "orgUnits".
    /// </summary>
    public OrgUnitModel OrgUnit { get; set; }

    /// <summary>
    /// The ID of the task
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// The task. Only included in the response if extend contains "tasks" or "laws".
    /// </summary>
    public TaskModel Task { get; set; }

    /// <summary>
    /// The interval of the delegation. Values are:
    /// - None -> the delegation is a one-time delegation or permanent
    /// - Daily -> the delegation is due daily
    /// - Weekly -> the delegation is due weekly
    /// - Monthly -> the delegation is due monthly
    /// - Yearly -> the delegation is due yearly
    /// </summary>
    public string Interval { get; set; }

    /// <summary>
    /// The type of the interval. Values are:
    /// Single -> the delegation is a one-time delegation
    /// Fixed -> the due date calculation after an inspection is based on the prevoius due date / start date
    /// Regular -> the due date calculation after an inspection is based on the last inspection date
    /// Permanent -> the delegation is a permanent delegation. No due dates are calulated.
    /// </summary>
    public string IntervalType { get; set; }

    /// <summary>
    /// The UTC date when the delegation is due
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The start date of the delegation. Only in
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// The status of the delegation. Values are:
    /// - Green -> the delegation is on time
    /// - Yellow -> the delegation is due soon
    /// - Red -> the delegation is overdue
    /// - PermanentOk -> the permanent delegation is ok
    /// - PermanentOverdue -> the permanent delegation is overdue
    /// </summary>
    public string DelegationStatus { get; set; }

    /// <summary>
    /// The comment of the delegation. Usually contains instructions for the responsible employee.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Comments specific to the site the delegation is in. Usually contains instructions for the responsible employee.
    /// </summary>
    public IList<SiteCommentModel> SiteComments { get; set; } = [];

    /// <summary>
    /// The created date of the delegation
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// The last updated date of the delegation
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// The warning period (number of days) for the delegation.
    /// When the due date of the delegation is within the warning period, the delegation status will be yellow.
    /// </summary>
    public int? WarningPeriod { get; set; }

    /// <summary>
    /// The interval count of the delegation. Only used if the interval is not "None".
    /// For example if the interval is "Weekly" and the interval count is 2, the delegation is due every 2 weeks.
    /// </summary>
    public int IntervalCount { get; set; }
}
