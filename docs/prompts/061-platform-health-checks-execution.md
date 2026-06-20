# Prompt 061 - Platform Health Checks Execution

## Prompt Number

061

## Date

2026-06-12

## Purpose

Execute the approved Platform Health Checks task.

## Full Prompt

APPROVED: EXECUTE Platform Health Checks

Additional Requirements:
- Add GET /health/live for process liveness only.
- Add GET /health/ready for Auth and Catalog database readiness.
- /health/live must not depend on database connectivity.
- /health/ready must not create databases, apply migrations, or change schema.
- Avoid package additions unless absolutely necessary.
- Keep this API/platform-only; do not change Domain, Application, CQRS, or module behavior.

## Status

EXECUTED

## Result Summary

Added API/platform-only health checks. `GET /health/live` uses a self health check for process liveness only and does not depend on database connectivity. `GET /health/ready` checks Auth and Catalog database readiness through `AuthDbContext` and `CatalogDbContext` using `Database.CanConnectAsync`. The readiness checks do not create databases, apply migrations, or change schema. No packages, migrations, Domain, Application, CQRS, Auth behavior, Catalog behavior, or module behavior changes were added. `dotnet build Ecommerce.sln` passed, and `dotnet test Ecommerce.sln --no-build` passed with 70 Catalog unit tests, 65 Auth unit tests, and 26 architecture tests.
