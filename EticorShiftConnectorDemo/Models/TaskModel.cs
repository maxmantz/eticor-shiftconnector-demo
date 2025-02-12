namespace EticorShiftConnectorDemo.Models;

internal class TaskModel
{
    /// <summary>
    /// The ID of the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The text of the task.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The dscirption of the task.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Whether or not th4e task is archived.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Whether or not the task is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The task's sources.
    /// </summary>
    public IList<SourceModel> Sources { get; set; } = [];
}
