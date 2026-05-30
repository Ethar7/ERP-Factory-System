# ERP Factory System

## Documentation

- [Workflow](docs/workflow.md)
- [ERD](docs/erd.md)
- [API](docs/api-design.md)
- [views](docs/Views.sql)

---

## Overview
ERP system for cement ionic factory (project-based production).

## C# API

The ASP.NET Core API code is in `src/ErpFactory.Api`.

Run it:

```powershell
dotnet restore .\ErpFactorySystem.slnx
dotnet run --project .\src\ErpFactory.Api\ErpFactory.Api.csproj
```

Swagger UI for testing:

```text
http://localhost:5101
https://localhost:7062
```

Update the SQL Server connection string in:

```text
src/ErpFactory.Api/appsettings.json
```

Default API base URL:

```text
api/
```
## Database Setup

Update the connection string in `src/ErpFactory.Api/appsettings.json` before running any commands.

**1. Create the Migration**

```powershell
dotnet ef migrations add InitialCreate --project .\src\ErpFactory.Api\ErpFactory.Api.csproj
```

**2. Apply the Migration**

```powershell
dotnet ef database update --project .\src\ErpFactory.Api\ErpFactory.Api.csproj
```

**3. Create the Views**

Open `docs/Views.sql` and run it manually against your SQL Server database using SSMS or Azure Data Studio.