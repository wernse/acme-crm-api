# Acme CRM API

Internal CRM API for Acme Retail (NZ). ASP.NET Core 8 + EF Core (SQL Server).

## Conventions
- All endpoints require auth (`[Authorize]`, role `Staff` unless stated otherwise)
- Queries are async, `AsNoTracking` for reads, and paged (max page size 200)
- Every PR: green CI (build + tests) and one approving review before merge

## Run
```
dotnet test
dotnet run --project src/AcmeExport.Api
```
