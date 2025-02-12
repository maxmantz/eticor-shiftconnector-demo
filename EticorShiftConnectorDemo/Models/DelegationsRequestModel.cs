namespace EticorShiftConnectorDemo.Models;

internal class DelegationsRequestModel : PageRequest
{
    /// <summary>
    /// If set, only delegations with the specified responsible will be returned.
    /// </summary>
    public int? ResponsibleId { get; set; }

    /// <summary>
    /// Whether or not to include task's that are archived.
    /// Archived tasks are irrelevant.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Whether or not to include disabled delegations.
    /// Disabled delegations are irrelevant
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Allows extending the results with related information
    /// </summary>
    public string[] Extend { get; set; } = [];
}
