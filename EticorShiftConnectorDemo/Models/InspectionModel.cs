namespace EticorShiftConnectorDemo.Models;

internal class InspectionModel
{
    /// <summary>
    /// The ID of the inspection
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The delegation
    /// </summary>
    public DelegationModel Delegation { get; set; }

    /// <summary>
    /// The inspection date
    /// </summary>
    public DateTime InspectionDate { get; set; }

    /// <summary>
    /// The inspector
    /// </summary>
    public EmployeeModel Inspector { get; set; }

    /// <summary>
    /// The name of the inspector.
    /// </summary>
    public string InspectorName { get; set; }

    /// <summary>
    /// The org unit of the inspection
    /// </summary>
    public OrgUnitModel OrgUnit { get; set; }

    /// <summary>
    /// The comment of the inspection
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// The type of the inspection. Values are:
    /// DueDate inspection. A new due date has been calculated for the delegation
    /// Intermediate inspection. No new due date has been calculated for the delegation
    /// </summary>
    public string InspectionType { get; set; }

    /// <summary>
    /// The UTC date when the inspection was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Whether the inspection is complete or not
    /// </summary>
    public bool IsComplete { get; set; }
}
