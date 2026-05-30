# ERP Factory System

## Documentation

- [Workflow](docs/workflow.md)
- [ERD](docs/erd.md)
- [Database](database/schema.sql)

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
