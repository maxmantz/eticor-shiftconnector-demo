// This is a demo console application to demonstrate the interaction of ShiftConnector with the Eticor API.


using EticorShiftConnectorDemo.Models;
using EticorShiftConnectorDemo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

# region init
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

JsonSerializerOptions jsonSerializerOptions = new()
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

using ILoggerFactory loggerFactory = LoggerFactory.Create(static builder => builder.AddConsole());
ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

EticorApiService service = new(config, jsonSerializerOptions, loggerFactory.CreateLogger<EticorApiService>());
#endregion

#region getting employee data
// we get the employee by the personnel number from the configuration
EmployeeModel employee = await service.GetEmployeeByEmailAsync(config["Email"]!);

logger.LogInformation($"Employee:");
logger.LogInformation(JsonSerializer.Serialize(employee, jsonSerializerOptions));
#endregion

#region getting delegations for employee
// we get the delegations for the employee by the employee id from the previous call
DelegationListRequestModel request = new()
{
    // we can use the page request properties to limit the result
    Offset = 0,
    Limit = 10,
    // we can filter the delegations by the responsible id
    ResponsibleId = employee.Id,
    // Note: The new API no longer supports IsArchived, IsDisabled, or Extend parameters
    // The API now returns full delegation details by default
    // We can optionally set OrderBy for sorting
    OrderBy = "duedate"
};

PageResult<DelegationModel> delegations = await service.GetDelegationsAsync(request);

logger.LogInformation($"Delegations:");
logger.LogInformation(JsonSerializer.Serialize(delegations, jsonSerializerOptions));
#endregion

#region getting orgUnits
// we get the orgUnits from the previous call. If we want all orgUnits we can use the following request
PageResult<OrgUnitModel> orgUnits = await service.GetOrgUnitsAsync(new OrgUnitRequestModel
{
    Offset = 0,
    Limit = 10
});

logger.LogInformation($"OrgUnits:");
logger.LogInformation(JsonSerializer.Serialize(orgUnits, jsonSerializerOptions));
#endregion

#region getting documents
// in delegations, documents can be attached to the delegation, the task or the. We can get those with the following requests
List<DocumentModel> documentsForDelegation = await service.GetDocumentsForDelegationAsync(delegations.Items.First().Id);
logger.LogInformation($"Documents for delegation:");
logger.LogInformation(JsonSerializer.Serialize(documentsForDelegation, jsonSerializerOptions));

PageResult<DocumentModel> documentsForTask = await service.GetDocumentsForTaskAsync(delegations.Items.First().TaskId);
logger.LogInformation($"Documents for task:");
logger.LogInformation(JsonSerializer.Serialize(documentsForTask, jsonSerializerOptions));

PageResult<DocumentModel> documentsForLaw = await service.GetDocumentsForLawAsync(delegations.Items.First().Task.Sources.First().Law.Id);
logger.LogInformation($"Documents for law:");
logger.LogInformation(JsonSerializer.Serialize(documentsForLaw, jsonSerializerOptions));

// usually laws have documents attached to them, we can get those with the following request
if (documentsForLaw.Items.Count > 0)
{
    DocumentModel documentFromLaw = await service.GetDocumentByIdAsync(documentsForLaw.Items.First().Id);
    await service.SaveDocumentAsync(documentFromLaw);
}
#endregion

#region creating an inspection
// to perform an inspection, the following request can be used
CreateInspectionModel validInspection = new()
{
    DelegationId = delegations.Items.First().Id,
    InspectorId = delegations.Items.First().ResponsibleId,
    InspectionDate = DateTime.UtcNow,
    InspectionType = 1,
    IsComplete = true,
    Comment = "Inspection completed" // must be 10 characters or longer
};

// we can also attach a document to the inspection
string filePath = Path.Join(Environment.CurrentDirectory, "FileAttachment.txt");
CreateDocumentModel docModel = new()
{
    Bytes = await File.ReadAllBytesAsync(filePath),
    FileName = Path.GetFileName(filePath),
    MimeType = "text/plain"
};

validInspection.Documents.Add(docModel);

InspectionModel inspection = await service.CreateInspectionAsync(validInspection);
logger.LogInformation($"Inspection:");
logger.LogInformation(JsonSerializer.Serialize(inspection, jsonSerializerOptions));
#endregion

#region filtering delegations by date
// we can filter the delegations by date. The new API uses StartDate and EndDate for filtering.
// To get delegations starting from today onwards, we use StartDate.
DateTime date = DateTime.UtcNow.Date;
request.StartDate = date;

delegations = await service.GetDelegationsAsync(request);
logger.LogInformation($"Delegations starting from {date}:");
logger.LogInformation(JsonSerializer.Serialize(delegations, jsonSerializerOptions));
#endregion

#region getting org unit by ID
// we can get the org unit by ID. The following request will get the org unit by ID
int? orgUnitId = delegations.Items.First().OrgUnitId;
OrgUnitModel orgUnit = await service.GetOrgUnitByIdAsync(orgUnitId.Value);

logger.LogInformation($"OrgUnit:");
logger.LogInformation(JsonSerializer.Serialize(orgUnit, jsonSerializerOptions));
#endregion

logger.LogInformation("Done");
