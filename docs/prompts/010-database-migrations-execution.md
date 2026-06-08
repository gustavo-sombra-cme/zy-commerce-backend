# Prompt 010: Database Migrations Execution

## Prompt Number

010

## Date

2026-06-08

## Purpose

Execute the approved Catalog database migrations plan.

## Full Prompt

APPROVED: EXECUTE

Execute the database migrations plan exactly.

Before execution:
- create/update docs/prompts/009-database-migrations-planning.md
- create docs/prompts/010-database-migrations-execution.md

Use:
- Manual EF Core migration
- SQL Server LocalDB
- ConnectionStrings:Catalog
- Migration name: InitialCatalogSchema
- Migration location: Persistence/Migrations

Do not create:
- startup auto-migration
- Docker
- new projects
- new modules
- domain/application/contracts changes
- business feature changes

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet ef migrations add InitialCatalogSchema --project src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj --startup-project src/Api/Ecommerce.Api/Ecommerce.Api.csproj --context CatalogDbContext --output-dir Persistence/Migrations
- dotnet ef database update --project src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Ecommerce.Catalog.Infrastructure.csproj --startup-project src/Api/Ecommerce.Api/Ecommerce.Api.csproj --context CatalogDbContext
- dotnet test Ecommerce.sln

Report:
- files changed
- migration files created
- database update result
- test results
- architecture test result
- deviations from plan

## Status

EXECUTED

## Result Summary

Execution started by creating the required prompt logs before migration setup and commands.
