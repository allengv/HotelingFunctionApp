# HotelingFunctionApp

An Azure Functions application that exposes available hoteling seats through an HTTP-triggered function. The application uses .NET 10 isolated worker, SQL Server for persistence, dependency injection for its business and data-access layers, and optional Azure Monitor OpenTelemetry export.

## Project structure

- `GetSeatsFunction.cs` - HTTP endpoint for retrieving seats.
- `BusinessLogic.cs` - Application logic and timing/logging around seat retrieval.
- `DataAccess.cs` - SQL Server access with transient retry handling.
- `SeatsModel.cs` - Seat response model.
- `Program.cs` - Isolated worker startup and dependency injection configuration.

## Prerequisites

- .NET 10 SDK
- Azure Functions Core Tools v4
- SQL Server or SQL Server Express
- Azurite, or another local Azure Storage emulator, for `AzureWebJobsStorage`

## Configuration

The function reads settings from environment variables. For local development, create or update `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
	"AzureWebJobsStorage": "UseDevelopmentStorage=true",
	"FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
	"SqlConnectionString": "<your SQL Server connection string>"
  }
}
```

Do not commit real credentials or production connection strings. `SqlConnectionString` must point to a database containing the `seats` table:

```sql
CREATE TABLE seats (
	SeatId int NOT NULL PRIMARY KEY,
	SeatName nvarchar(255) NOT NULL
);
```

## Run locally

1. Start SQL Server and ensure the configured database and `seats` table exist.
2. Start Azurite if `AzureWebJobsStorage` uses `UseDevelopmentStorage=true`.
3. Start the function app from the repository directory:

   ```powershell
   dotnet run
   ```

The default HTTP endpoint is:

```text
GET http://localhost:7071/api/GetSeatsFunction
```

The function also accepts `POST` requests. A successful response is a JSON array containing `SeatId` and `SeatName` values. Database or application failures return HTTP 500.

## Observability

Set `APPLICATIONINSIGHTS_CONNECTION_STRING` in the deployment environment to enable Azure Monitor OpenTelemetry export. Application logs include database retrieval duration, retry attempts, and errors.

## Build

```powershell
dotnet build
```
