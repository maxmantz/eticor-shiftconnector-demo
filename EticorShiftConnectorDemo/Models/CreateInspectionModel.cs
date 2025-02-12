namespace EticorShiftConnectorDemo.Models;

internal class CreateInspectionModel
{
    /// <summary>
    /// ID of the delegation to inspect
    /// </summary>
    public int DelegationId { get; set; }

    /// <summary>
    /// The comment of the inspection. Must be at least 10 characters long.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// The date of the inspection. If null, the current date will be used.
    /// </summary>
    public DateTime? InspectionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The employee ID of the inspector. Usually the ID of the responsible employee.
    /// </summary>
    public int InspectorId { get; set; }

    /// <summary>
    /// Whether the inspection is complete or not.
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Options are:
    /// 1 - DueDate -> a new due date will be calculated
    /// 2 - Intermediate -> no new due date will be calculated
    /// </summary>
    public int InspectionType { get; set; } = 1;

    /// <summary>
    /// Documents to attach to the inspection
    /// </summary>
    public IList<CreateDocumentModel> Documents { get; set; } = [];
}
