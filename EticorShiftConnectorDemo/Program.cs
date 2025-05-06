// This is a demo console application to demonstrate the interaction of ShiftConnector with the Eticor API.


using EticorShiftConnectorDemo.Models;
using EticorShiftConnectorDemo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

# region init
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var service = new EticorApiService(config, jsonSerializerOptions, loggerFactory.CreateLogger<EticorApiService>());
#endregion

#region getting employee data
// we get the employee by the personnell number from the configuration
var employee = await service.GetEmployeeByPersonnellNumberAsync(config["PersonnellNumber"]!);

logger.LogInformation($"Employee:");
logger.LogInformation(JsonSerializer.Serialize(employee, jsonSerializerOptions));
#endregion

#region getting delegations for employee
// we get the delegations for the employee by the employee id from the previous call
var request = new DelegationsRequestModel
{
    // we can use the page request properties to limit the result
    Offset = 0,
    Limit = 10,
    // we can filter the delegations by the responsible id
    ResponsibleId = employee.Id,
    IsArchived = false, // don't include delegations of archived tasks
    IsDisabled = false, // don't include delegations that are disabled
    // extending the response with the employees, orgUnits and laws gives us the information about related entities
    Extend = ["employees", "orgUnits", "laws"]
};

var delegations = await service.GetDelegationsAsync(request);

logger.LogInformation($"Delegations:");
logger.LogInformation(JsonSerializer.Serialize(delegations, jsonSerializerOptions));
#endregion

#region getting orgUnits
// we get the orgUnits from the previous call. If we want all orgUnits we can use the following request
var orgUnits = await service.GetOrgUnitsAsync(new OrgUnitRequestModel
{
    Offset = 0,
    Limit = 10
});

logger.LogInformation($"OrgUnits:");
logger.LogInformation(JsonSerializer.Serialize(orgUnits, jsonSerializerOptions));
#endregion

#region getting documents
// in delegations, documents can be attached to the delegation, the task or the. We can get those with the following requests
var documentsForDelegation = await service.GetDocumentsForDelegationAsync(delegations.Items.First().Id);
logger.LogInformation($"Documents for delegation:");
logger.LogInformation(JsonSerializer.Serialize(documentsForDelegation, jsonSerializerOptions));

var documentsForTask = await service.GetDocumentsForTaskAsync(delegations.Items.First().TaskId);
logger.LogInformation($"Documents for task:");
logger.LogInformation(JsonSerializer.Serialize(delegations.Items.First().TaskId, jsonSerializerOptions));

var documentsForLaw = await service.GetDocumentsForLawAsync(delegations.Items.First().Task.Sources.First().Law.Id);
logger.LogInformation($"Documents for law:");
logger.LogInformation(JsonSerializer.Serialize(documentsForLaw, jsonSerializerOptions));

// usually laws have documents attached to them, we can get those with the following request
if (documentsForLaw.Items.Count > 0)
{
    var documentFromLaw = await service.GetDocumentByIdAsync(documentsForLaw.Items.First().Id);
    await service.SaveDocumentAsync(documentFromLaw);
}
#endregion

#region creating an inspection
// to perform an inspection, the following request can be used
var validInspection = new CreateInspectionModel
{
    DelegationId = delegations.Items.First().Id,
    InspectorId = delegations.Items.First().ResponsibleId,
    InspectionDate = DateTime.UtcNow,
    InspectionType = 1,
    IsComplete = true,
    Comment = "Inspection completed" // must be 10 characters or longer
};

// we can also attach a document to the inspection
var filePath = Path.Join(Environment.CurrentDirectory, "FileAttachment.txt");
var docModel = new CreateDocumentModel
{
    Bytes = await File.ReadAllBytesAsync(filePath),
    FileName = Path.GetFileName(filePath),
    MimeType = "text/plain"
};

validInspection.Documents.Add(docModel);

var inspection = await service.CreateInspectionAsync(validInspection);
logger.LogInformation($"Inspection:");
logger.LogInformation(JsonSerializer.Serialize(inspection, jsonSerializerOptions));
#endregion

#region filtering delegations by date
// we can filter the delegations by date. The following request will get all delegations that have changed today.
var date = DateTime.UtcNow.Date;
request.NewerThan = date;

delegations = await service.GetDelegationsAsync(request);
logger.LogInformation($"Delegations newer than {date}:");
logger.LogInformation(JsonSerializer.Serialize(delegations, jsonSerializerOptions));
#endregion

logger.LogInformation("Done");
