# Project Status

## Snapshot

Date: 2026-06-08

The repository is a .NET 9 ASP.NET Core backend for an enterprise e-commerce system. It follows Clean Architecture First, Modular Monolith, Module Isolation, CQRS, and thin controller rules.

Agent operating rules are split between a short `AGENT.md` router and detailed files under `instructions/`.

**Project Memory Files:**

- `docs/project/PROJECT_STATUS.md` - Current implementation snapshot
- `docs/project/AI_HANDOFF.md` - Constraints and architecture for new sessions
- `docs/project/ROADMAP.md` - Completed and candidate work
- `docs/project/NEXT_SESSION.md` - Fast resume guide (< 5 minutes to read)

## Solution Structure

Current approved projects:

- `src/Api/Ecommerce.Api`
- `src/BuildingBlocks/Ecommerce.BuildingBlocks.Domain`
- `src/BuildingBlocks/Ecommerce.BuildingBlocks.Application`
- `src/BuildingBlocks/Ecommerce.BuildingBlocks.Infrastructure`
- `src/Modules/Catalog/Ecommerce.Catalog.Domain`
- `src/Modules/Catalog/Ecommerce.Catalog.Application`
- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure`
- `src/Modules/Catalog/Ecommerce.Catalog.Contracts`
- `src/Modules/Auth/Ecommerce.Auth.Domain`
- `src/Modules/Auth/Ecommerce.Auth.Application`
- `src/Modules/Auth/Ecommerce.Auth.Infrastructure`
- `src/Modules/Auth/Ecommerce.Auth.Contracts`
- `tests/ArchitectureTests/Ecommerce.ArchitectureTests`
- `tests/UnitTests/Ecommerce.Catalog.UnitTests`
- `tests/UnitTests/Ecommerce.Auth.UnitTests`

## Active Modules

### Catalog

Catalog is the only module with business behavior.

Implemented product capabilities:

- Create Product
- Get Product By Id
- Search/List Products with pagination
- Deactivate Product

Catalog uses:

- DDD aggregate behavior for `Product`
- CQRS with MediatR
- FluentValidation
- EF Core persistence
- SQL Server LocalDB
- Manual EF Core migration: `InitialCatalogSchema`
- Query-side read model for product search

### Auth

Auth exists as a Phase 1 skeleton only.

Current Auth projects:

- `Ecommerce.Auth.Domain`
- `Ecommerce.Auth.Application`
- `Ecommerce.Auth.Infrastructure`
- `Ecommerce.Auth.Contracts`
- `Ecommerce.Auth.UnitTests`

Auth intentionally does not contain:

- User aggregate
- JWT implementation
- refresh tokens
- password hashing
- DbContext
- migrations
- API endpoints
- roles or permissions

## API Status

Catalog endpoints are implemented through controllers under:

- `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`

Current product routes:

- `POST /api/catalog/products`
- `GET /api/catalog/products/{productId}`
- `GET /api/catalog/products`
- `DELETE /api/catalog/products/{productId}`

Swagger/OpenAPI is enabled for local Development only.

## Database Status

Catalog has EF Core persistence and one approved migration:

- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260608111338_InitialCatalogSchema.cs`

Local development uses:

- `ConnectionStrings:Catalog`
- SQL Server LocalDB
- `(localdb)\mssqllocaldb`

No Auth database objects exist.

## Error Handling

The API has global exception middleware:

- Validation failures return `ValidationProblemDetails`.
- Duplicate SKU returns conflict.
- Not found support returns not found where used.
- Unknown exceptions return generic server errors.

Controllers keep transport-level checks, such as explicit `Guid.Empty` handling.

## Architecture Rules

Architecture tests enforce:

- Domain projects do not reference Application, Infrastructure, Contracts, or Api.
- Application projects do not reference Infrastructure or Api.
- Infrastructure projects do not reference Api.
- BuildingBlocks projects do not reference Catalog or Auth.
- Catalog and Auth do not reference each other.
- Only approved projects exist.
- Only approved modules exist.
- No Bootstrapper or Shared projects exist.

Agent rule files:

- `AGENT.md` is the short router.
- `instructions/00-role-and-stack.md` contains role, stack, strategy, repository, and active module rules.
- `instructions/01-execution-and-planning.md` contains state, execution lock, and planning rules.
- `instructions/02-architecture-and-modules.md` contains Clean Architecture and module rules.
- `instructions/03-cqrs-database-testing-security.md` contains CQRS, database, testing, architecture test, and security rules.
- `instructions/04-documentation-and-memory.md` contains prompt logging, project memory, AGENT replacement, ADR, and learning journal rules.
- `instructions/05-completion.md` contains self-review and completion rules.

## Test Status

Latest known execution after Auth skeleton:

- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 47 passed
- Architecture tests: 11 passed
- Auth unit test project exists but contains no tests yet

## Intentionally Absent

The repository intentionally does not currently include:

- Bootstrapper project
- Shared project
- microservices
- event bus
- distributed transactions
- startup auto-migrations
- Docker setup
- Customers module
- Orders module
- Inventory module
- Auth business features
