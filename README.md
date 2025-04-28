# semester-project

## Database migrations

### Prerequisites
- Postgres database is running on localhost:5432, with username:password = postgres:postgres
- Install ef core tool `dotnet tool install --global dotnet-ef`

1. Create migration
```
dotnet ef migrations add <migration name>
```

2. Update Database
```
dotnet ef database update
```
