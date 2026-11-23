# DNP forum

The Web API uses Entity Framework Core with SQLite. The Blazor app talks to the API. The older CLI still uses its JSON file repositories.

## Create the database

From the solution folder in PowerShell, install the EF tool if needed:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.15
```

Apply the migration to a database in the solution folder:

```powershell
$database = Join-Path (Get-Location) 'forum.db'
dotnet ef database update --project Server/EfcRepositories --startup-project Server/EfcRepositories --connection "Data Source=$database"
```

## Run

From the same solution folder, start the API:

```powershell
dotnet run --project Server/WebAPI/WebAPI.csproj
```

In another terminal, from the same folder, start the client:

```powershell
dotnet run --project Client/BlazorApp/BlazorApp.csproj
```

The API uses `forum.db` relative to its working directory. In Rider, set the API run configuration's working directory to the solution folder so it finds the same database. The old JSON files are not imported automatically or deleted. The SQLite database starts empty; create a user before creating posts and comments.

The SQLite database and the JSON data files are ignored by Git. Passwords are stored as plain text in this classroom example; do not use real passwords.
