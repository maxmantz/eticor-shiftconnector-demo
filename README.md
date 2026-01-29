# Eticor Shift Connector Demo

## Overview

The Eticor Shift Connector Demo is a .NET Core application designed to demonstrate the integration capabilities of the Eticor ShiftConnector interface. This project showcases how to connect, retrieve, and manipulate data from the Eticor system.

**⚠️ API Version Update**: This application has been updated to use the new **v2/public API** endpoints. See [UPGRADE_GUIDE.md](UPGRADE_GUIDE.md) for detailed migration information.

## Features

- Connect to the Eticor system using OAuth 2.0 client credentials
- Retrieve employee data by personnel number
- Query delegations with advanced filtering options
- Access organizational units
- Retrieve documents from delegations and tasks
- Create inspections via the Eticor API

## Prerequisites

- .NET Core SDK 8
- Visual Studio 2019 or later / Visual Studio Code
- Eticor system credentials

## Getting Started

1. **Clone the repository:**

   ```sh
   git clone https://github.com/yourusername/EticorShiftConnectorDemo.git
   cd EticorShiftConnectorDemo
   ```

2. **Restore dependencies:**

   ```sh
   dotnet restore
   ```

3. **Build the project:**

   ```sh
   dotnet build
   ```

4. **Run the application:**
   ```sh
   dotnet run
   ```

## Configuration

Update the `appsettings.json` file with your Eticor system credentials and other necessary configurations.

### Configuration Example

```json
{
  "Authority": "https://devauth.eticor-portal.com/realms/eticor-dev",
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret",
  "ApiRoot": "https://devapi.eticor-portal.com/v2/",
  "CustomerId": "your-customer-id",
  "PersonnelNumber": "your-personnel-number"
}
```

**Note**: The configuration key `PersonnelNumber` has been corrected from the previous typo `PersonnellNumber`.

## Recent Changes (API v2 → v2/public Migration)

This application has been updated to work with the new Eticor public API. Key changes include:

### Endpoint Updates

- All endpoints now use the `/public` prefix (e.g., `v2/public/employees` instead of `v2/employees`)
- Fixed typo: `personellNumber` → `personnelNumber`

### Model Updates

- `DelegationsRequestModel` replaced with `DelegationListRequestModel`
- Removed unsupported parameters: `IsArchived`, `IsDisabled`, `Extend`, `NewerThan`
- Added new filtering parameters: `StartDate`, `EndDate`, `OrderBy`, `Descending`, and more

### Behavioral Changes

- Employee endpoint now returns extended data by default (orgUnits, permissions, deputies)
- The `extend` parameter is no longer needed
- Laws endpoint is not available in the public API

### For Complete Migration Details

See [UPGRADE_GUIDE.md](UPGRADE_GUIDE.md) for comprehensive migration instructions, including:

- Detailed breaking changes
- Complete endpoint mapping
- Code migration examples
