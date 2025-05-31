# semester-project

## Geting Started

To run the project, you need to have the following prerequisites installed:

- .NET 9.0
- Docker (for running the PostgreSQL database)

Before running the project, ensure you have the PostgreSQL running.

You can run the PostgreSQL database using Docker with the following command in root of the repository:
```bash
docker compose up -d
```

Then you can navigate to the directory `\semester-project\src\HeatManager` and run the application by typing:
```bash
dotnet run
```


After you are done, remember to stop the database with:
```bash
docker compose down
```

> Note: Applying database migrations isn't necessary because the application sets up the database automatically on first run with mock data.

## Database migrations

### Prerequisites
- Postgres database is running on localhost:5432, with username:password = postgres:postgres
- Install ef core tool `dotnet tool install --global dotnet-ef`

1. Create migration 

```
# In HeatManager.Core directory
> dotnet ef migrations add <migration name>
```

2. Update Database

```
# In HeatManager.Core directory
> dotnet ef database update
```