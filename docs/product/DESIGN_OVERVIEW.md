# Design Overview

Last updated: 2026-06-19

## Implemented Backend Design

The backend is a Clean Architecture modular monolith. The API project is the composition/transport layer, modules own their business behavior, and Infrastructure implements persistence behind Application abstractions.

Reference structure:

- API: `src/Api/Ecommerce.Api`
- BuildingBlocks: `src/BuildingBlocks`
- Catalog: `src/Modules/Catalog`
- Auth: `src/Modules/Auth`
- Orders: `src/Modules/Orders`
- Architecture tests: `tests/ArchitectureTests/Ecommerce.ArchitectureTests`

Dependency rules are enforced by tests such as `DependencyRuleTests.cs`, `ProjectStructureTests.cs`, and `ApiAuthorizationTests.cs`.

## Implemented Domain and Module Design

Catalog owns product lifecycle. The aggregate is `Product`, with `ProductId`, `Sku`, and `ProductName` value objects. Product price is now aggregate state. References: `src/Modules/Catalog/Ecommerce.Catalog.Domain/Products/Product.cs`, `src/Modules/Catalog/Ecommerce.Catalog.Domain/Products/Sku.cs`.

Auth owns user identity, email, password hash, login validation, and JWT access-token generation. References: `src/Modules/Auth/Ecommerce.Auth.Domain/Users/User.cs`, `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Security/JwtAccessTokenGenerator.cs`.

Orders owns order history, order lines, buyer scoping, totals, and product snapshots. References: `src/Modules/Orders/Ecommerce.Orders.Domain/Orders/Order.cs`, `src/Modules/Orders/Ecommerce.Orders.Domain/Orders/OrderLine.cs`.

## Implemented CQRS Design

Writes use commands, handlers, and validators:

- Catalog create/update/deactivate/reactivate: `src/Modules/Catalog/Ecommerce.Catalog.Application/Products`.
- Auth register/login: `src/Modules/Auth/Ecommerce.Auth.Application/Users`.
- Orders create: `src/Modules/Orders/Ecommerce.Orders.Application/Orders/CreateOrder`.

Reads use queries, handlers, and DTOs:

- Catalog get/search: `GetProductByIdQuery`, `SearchProductsQuery`.
- Orders list/get: `ListOrdersForBuyerQuery`, `GetOrderByIdQuery`.

Controllers dispatch through MediatR `ISender`. References: `ProductsController.cs`, `AuthUsersController.cs`, `OrdersController.cs`.

## Implemented Persistence Design

Each module owns its DbContext and migrations:

- Catalog: `CatalogDbContext`, `CatalogReadDbContext`, `InitialCatalogSchema`, `AddProductPrice`.
- Auth: `AuthDbContext`, `InitialAuthSchema`.
- Orders: `OrdersDbContext`, `InitialOrdersSchema`.

Catalog search uses `ProductSearchReadModel` as a keyless, infrastructure-only read model. Reference: `docs/decisions/ADR-001-product-search-read-model.md`.

## Implemented API and Platform Design

The API layer includes:

- Controllers under `src/Api/Ecommerce.Api/Controllers`.
- Global exception handling in `ExceptionHandlingMiddleware`.
- Correlation ID behavior in `CorrelationIdMiddleware`.
- Structured request logging in `RequestLoggingMiddleware`.
- Health readiness checks in `DbContextHealthCheck`.
- MCP adapter under `src/Api/Ecommerce.Api/Mcp`.
- Assistant orchestration under `src/Api/Ecommerce.Api/Assistant` with transport in `src/Api/Ecommerce.Api/Controllers/Assistant`.

MCP follows ADR-003: API-layer adapter, protected endpoint, explicit allowlist, dispatch via `ISender`.

Assistant orchestration follows ADR-004: API-layer deterministic read-only orchestration, protected endpoint, explicit capability allowlist, Orders owner scoping through JWT `sub`, and dispatch through existing read-side CQRS queries via `ISender`.

## Implemented Frontend Design

The frontend is Angular v22 with standalone components and lazy feature routes. References: `../zy-commerce-frontend/AGENT.md`, `../zy-commerce-frontend/src/app/app.routes.ts`.

Layout:

- `src/app/core`: auth, config, guards, interceptors, logging, layout.
- `src/app/shared`: reusable UI primitives.
- `src/app/features`: auth, catalog, cart, orders, MCP assistant.
- `src/app/mcp`: MCP interfaces, registry, HTTP client, confirmation service.

Runtime config is loaded from `src/assets/config/runtime-config.json` through `RuntimeConfigService`. Runtime config is public and must not contain secrets. References: `../zy-commerce-frontend/docs/project/SECURITY.md`.

## Partially Implemented / Skeleton Only

- Frontend MCP assistant route and MCP services exist, but the docs describe real MCP execution as not fully implemented. References: `../zy-commerce-frontend/docs/project/CURRENT_STATE.md`, `../zy-commerce-frontend/src/app/features/mcp-assistant`.
- Backend assistant is implemented for deterministic Phase 1 only; external LLM-backed orchestration is not implemented.

## Intentionally Absent

- Backend does not use microservices, distributed transactions, event bus, Bootstrapper, Shared project, or startup auto-migrations.
- Backend assistant does not perform writes, call raw SQL, expose cross-user data, or use an external LLM provider in Phase 1.
- Frontend does not call AI provider APIs directly from the browser and does not use MCP for login/register.

## Unknown / Not Verified

- No runtime deployment topology is documented here beyond local Development/LocalDB references.
- No fresh runtime smoke test was performed for this documentation pack.
