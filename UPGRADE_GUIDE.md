# Eticor API v2 to v2/public Migration Guide

This guide provides comprehensive information for migrating from the Eticor API v2 to the new v2/public API endpoints.

## Table of Contents

1. [Overview](#overview)
2. [Breaking Changes](#breaking-changes)
3. [Endpoint Changes](#endpoint-changes)
4. [Model Changes](#model-changes)
5. [Behavioral Changes](#behavioral-changes)
6. [Migration Steps](#migration-steps)
7. [Code Examples](#code-examples)

## Overview

The new Eticor API introduces a `/public` prefix to all endpoints and includes several important changes to endpoint paths, parameter names, and data structures. The new API also provides enhanced data by default without requiring explicit "extend" parameters.

### Key Benefits of the New API

- **Richer default responses**: Extended data (orgUnits, permissions, deputies) is now included by default
- **Corrected naming**: Fixed typo in "personnelNumber" (was "personellNumber")
- **More filtering options**: Additional query parameters for fine-grained data filtering
- **Better sorting**: Built-in OrderBy and Descending parameters for delegations

### Backward Compatibility

The new API maintains backward compatibility by supporting both old and new endpoint paths during a transition period. However, it is recommended to migrate to the new `/public` endpoints for future compatibility.

## Breaking Changes

### 1. URL Path Structure

All endpoints now require the `/public` prefix in the path.

**Old API:**

```
https://{baseUrl}/employees/personellNumber/{personnelNumber}
```

**New API:**

```
https://{baseUrl}/v2/public/employees/{personnelNumber}
```

### 2. Personnel Number Endpoint

The personnel number endpoint has been restructured and the typo has been corrected.

**Old API:**

- Path: `v2/employees/personellNumber/{employeePersonellNumber}`
- Supported `extend` query parameter for related entities

**New API:**

- Path: `v2/public/employees/{personnelNumber}`
- No longer needs `extend` parameter - always returns full details including:
  - Deputy information
  - Permissions
  - OrgUnits with access levels

### 3. Delegations Request Model

The request model for delegations has changed significantly.

**Removed Parameters:**

- `IsArchived` - No longer supported
- `IsDisabled` - No longer supported
- `Extend` - No longer needed (ignored if sent)
- `NewerThan` - Replaced by `StartDate` and `EndDate`

**New Parameters:**

- `DelegationId` - Filter by specific delegation ID
- `EmployeeId` - Filter by employee
- `StartDate` - Filter by start date
- `EndDate` - Filter by end date
- `OrgUnitPath` - Filter by organization unit path
- `SourcePath` - Filter by source path
- `SearchQuery` - Full-text search
- `TagIds` - Filter by tags
- `UsedInTaskPackages` - Filter delegations used in task packages
- `RiskFactor` - Filter by risk level
- `DelegationDateType` - Specify date type for filtering
- `OrderBy` - Sort results by field (default: "duedate")
- `Descending` - Sort in descending order

### 4. Laws Endpoint

The `/public/laws` endpoint is **not available** in the new public API. If you need law-related data, it must be accessed through other endpoints (e.g., task sources include law information).

## Endpoint Changes

### Complete Endpoint Mapping

| Old API Endpoint                                 | New API Endpoint                                 | Notes                                    |
| ------------------------------------------------ | ------------------------------------------------ | ---------------------------------------- |
| `v2/employees/personellNumber/{personnelNumber}` | `v2/public/employees/{personnelNumber}`          | Typo corrected, extend parameter removed |
| `v2/delegations`                                 | `v2/public/delegations`                          | Different parameters available           |
| `v2/orgUnits`                                    | `v2/public/orgUnits`                             | Same functionality                       |
| `v2/orgUnits/{id}`                               | `v2/public/orgUnits/{id}`                        | Same functionality                       |
| `v2/tasks/{taskId}/documents`                    | `v2/public/tasks/{taskId}/documents`             | Same functionality                       |
| `v2/delegations/{delegationId}/documents`        | `v2/public/delegations/{delegationId}/documents` | Same functionality                       |
| `v2/documents/{documentId}`                      | `v2/public/documents/{documentId}`               | Same functionality                       |
| `v2/inspections`                                 | `v2/public/inspections`                          | Same functionality                       |
| `v2/laws/{lawId}/documents`                      | `v2/public/laws/{lawId}/documents`               | Same functionality                       |

## Model Changes

### DelegationsRequestModel → DelegationListRequestModel

**Old Model (DelegationsRequestModel):**

```csharp
internal class DelegationsRequestModel : PageRequest
{
    public int? ResponsibleId { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDisabled { get; set; }
    public string[] Extend { get; set; } = [];
    public DateTime? NewerThan { get; set; }
}
```

**New Model (DelegationListRequestModel):**

```csharp
internal class DelegationListRequestModel : PageRequest
{
    public int? DelegationId { get; set; }
    public int? TaskId { get; set; }
    public int? ResponsibleId { get; set; }
    public int? ControllerId { get; set; }
    public int? DeputyId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? OrgUnitPath { get; set; }
    public string? SourcePath { get; set; }
    public int? RoleId { get; set; }
    public int? TaskPackageId { get; set; }
    public string? SearchQuery { get; set; }
    public string[]? TagIds { get; set; }
    public bool? UsedInTaskPackages { get; set; }
    public int? RiskFactor { get; set; }
    public string? DelegationDateType { get; set; }
    public string? OrderBy { get; set; } = "duedate";
    public bool? Descending { get; set; }
}
```

## Behavioral Changes

### 1. Extended Data by Default

The new API returns extended information without requiring the `extend` parameter.

**Employee Endpoint:**

- **Old API**: Required `?extend=orgUnits,permissions` to get full data
- **New API**: Always returns orgUnits (with accessLevel), permissions, and deputy information

### 2. Parameter Handling

The new API is more tolerant of unsupported parameters:

- Sending old parameters (like `IsArchived`, `Extend`) to the new API won't cause errors
- Unsupported parameters are simply ignored
- This allows for gradual migration without breaking existing code

### 3. Response Structure

The response structure for most endpoints remains the same, but the data is more complete:

**Old API Response (with extend parameter):**

```json
{
  "id": 7,
  "firstName": "User",
  "lastName": "Name",
  "personnellNumber": "PN",
  "orgUnits": [],
  "permissions": []
}
```

**New API Response (automatic):**

```json
{
  "id": 7,
  "firstName": "User",
  "lastName": "Name",
  "personnelNumber": "PN",
  "orgUnits": [
    {
      "accessLevel": "Admin",
      "id": 70,
      "name": "OrgUnit"
    }
  ],
  "permissions": ["access.administrator"],
  "activeDelegationOrgUnitIds": [70, 81, 85]
}
```

## Migration Steps

### Step 1: Update Configuration

1. Update your `appsettings.json` with the new API endpoints and credentials
2. Correct the typo: `PersonnellNumber` → `PersonnelNumber`

### Step 2: Update Endpoint Constants

Update your endpoint constants to include the `/public` prefix:

```csharp
internal static class Endpoints
{
    // Old
    // public const string EmployeeByPersonnellNumber = "employees/personellNumber";

    // New
    public const string EmployeeByPersonnelNumber = "public/employees";
    public const string Delegations = "public/delegations";
    public const string OrgUnits = "public/orgUnits";
    public const string Tasks = "public/tasks";
    public const string Documents = "public/documents";
    public const string Inspections = "public/inspections";
}
```

### Step 3: Update Method Names

Fix the typo in method names and parameter names:

```csharp
// Old
public async Task<EmployeeModel> GetEmployeeByPersonnellNumberAsync(string personnellNumber)

// New
public async Task<EmployeeModel> GetEmployeeByPersonnelNumberAsync(string personnelNumber)
```

### Step 4: Update Request Models

Replace `DelegationsRequestModel` with `DelegationListRequestModel`:

```csharp
// Old
DelegationsRequestModel request = new DelegationsRequestModel
{
    ResponsibleId = employee.Id,
    IsArchived = false,
    IsDisabled = false,
    Extend = ["employees", "orgUnits", "laws"]
};

// New
DelegationListRequestModel request = new DelegationListRequestModel
{
    ResponsibleId = employee.Id,
    OrderBy = "duedate"
    // IsArchived, IsDisabled, and Extend are no longer used
};
```

### Step 5: Update Date Filtering

Replace `NewerThan` with `StartDate` and `EndDate`:

```csharp
// Old
request.NewerThan = DateTime.UtcNow.Date;

// New
request.StartDate = DateTime.UtcNow.Date;
// Optionally also set EndDate for a date range
```

### Step 6: Handle Removed Endpoints

Remove or comment out code that uses unavailable endpoints:

```csharp
// The laws endpoint is not available in the new public API
// Comment out or remove code that calls GetDocumentsForLawAsync
// PageResult<DocumentModel> documentsForLaw = await service.GetDocumentsForLawAsync(lawId);
```

### Step 7: Remove Extend Parameters

Remove code that sends `extend` parameters (they're no longer needed):

```csharp
// Old
string requestPath = $"{Endpoints.EmployeeByPersonnelNumber}/{personnelNumber}?extend=orgUnits&extend=permissions";

// New
string requestPath = $"{Endpoints.EmployeeByPersonnelNumber}/{personnelNumber}";
// Extended data is returned automatically
```

### Step 8: Update Extension Methods (if needed)

Ensure your `ToQueryParameters` extension method handles nullable arrays properly:

```csharp
public static string ToQueryParameters(this object obj)
{
    var properties = obj.GetType().GetProperties();
    StringBuilder query = new StringBuilder();

    foreach (var property in properties)
    {
        var value = property.GetValue(obj);
        if (value == null)
        {
            continue; // Skip null values
        }

        if (value is IEnumerable enumerable and not string)
        {
            foreach (var item in enumerable)
            {
                if (item != null)
                {
                    query.Append($"{property.Name}={item}&");
                }
            }
        }
        else if (property.PropertyType == typeof(DateTime?) && value is DateTime dateTime)
        {
            query.Append($"{property.Name}={dateTime:yyyy-MM-ddTHH:mm:ssZ}&");
        }
        else
        {
            query.Append($"{property.Name}={value}&");
        }
    }

    return query.ToString().TrimEnd('&');
}
```

## Code Examples

### Complete Migration Example

**Before (Old API):**

```csharp
// Configuration
config["PersonnellNumber"]  // With typo

// Get employee
EmployeeModel employee = await service.GetEmployeeByPersonnellNumberAsync(
    config["PersonnellNumber"]!
);

// Get delegations
DelegationsRequestModel request = new DelegationsRequestModel
{
    Offset = 0,
    Limit = 10,
    ResponsibleId = employee.Id,
    IsArchived = false,
    IsDisabled = false,
    Extend = ["employees", "orgUnits", "laws"],
    NewerThan = DateTime.UtcNow.Date
};

PageResult<DelegationModel> delegations = await service.GetDelegationsAsync(request);
```

**After (New API):**

```csharp
// Configuration
config["PersonnelNumber"]  // Typo fixed

// Get employee - now returns extended data automatically
EmployeeModel employee = await service.GetEmployeeByPersonnelNumberAsync(
    config["PersonnelNumber"]!
);

// Get delegations
DelegationListRequestModel request = new DelegationListRequestModel
{
    Offset = 0,
    Limit = 10,
    ResponsibleId = employee.Id,
    StartDate = DateTime.UtcNow.Date,
    OrderBy = "duedate"
    // IsArchived, IsDisabled, Extend, and NewerThan are no longer used
};

PageResult<DelegationModel> delegations = await service.GetDelegationsAsync(request);
```
