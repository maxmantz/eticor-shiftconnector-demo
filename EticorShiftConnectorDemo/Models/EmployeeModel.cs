namespace EticorShiftConnectorDemo.Models;

internal class EmployeeModel
{
    /// <summary>
    /// The ID of the employee
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The salutation of the employee
    /// </summary>
    public string Salutation { get; set; }

    /// <summary>
    /// The title of the employee
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The first name of the employee
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The last name of the employee
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// The email of the employee
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The personnell number of the employee
    /// </summary>
    public string PersonnellNumber { get; set; }
}
