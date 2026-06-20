# Product Requirements

Last updated: 2026-06-19

Source repositories:

- Backend: `c:\ZippyYum\Learning\zy-commerce-backend`
- Frontend: `c:\ZippyYum\Learning\zy-commerce-frontend`

## Implemented

### Platform and Architecture

- The backend is a .NET 9 ASP.NET Core modular monolith using Clean Architecture, CQRS, MediatR, FluentValidation, EF Core, SQL Server LocalDB, and xUnit. References: `docs/project/PROJECT_STATUS.md`, `src/Api/Ecommerce.Api/Program.cs`, `src/Modules/*`.
- Backend modules are isolated into Catalog, Auth, and Orders, each with Domain, Application, Infrastructure, and Contracts projects. References: `docs/project/PROJECT_STATUS.md`, `tests/ArchitectureTests/Ecommerce.ArchitectureTests/ProjectStructureTests.cs`.
- The frontend is an Angular v22 app with standalone components, lazy route boundaries, strict TypeScript, runtime config, REST API clients, HTTP interceptors, and feature folders. References: `../zy-commerce-frontend/AGENT.md`, `../zy-commerce-frontend/docs/project/ARCHITECTURE.md`, `../zy-commerce-frontend/src/app/app.routes.ts`.

### Catalog

- Backend supports product create, get by id, search/list, update details, deactivate, and reactivate. References: `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`, `src/Modules/Catalog/Ecommerce.Catalog.Application/Products`.
- Catalog product create accepts non-negative `price`, stores it on the Product aggregate, persists `decimal(18,2)`, and returns price from search/details. References: `src/Modules/Catalog/Ecommerce.Catalog.Domain/Products/Product.cs`, `src/Modules/Catalog/Ecommerce.Catalog.Contracts/Products/CreateProductRequest.cs`, `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260618090000_AddProductPrice.cs`.
- Catalog search uses an infrastructure read model to preserve DDD value objects while keeping server-side filtering. References: `docs/decisions/ADR-001-product-search-read-model.md`, `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Products/ProductSearchReadModel.cs`.
- Frontend has Catalog browse/search and product details routes and API client support. References: `../zy-commerce-frontend/src/app/features/catalog/catalog.routes.ts`, `../zy-commerce-frontend/src/app/features/catalog/product-details.routes.ts`, `../zy-commerce-frontend/src/app/features/catalog/data/catalog-api.client.ts`.

### Auth

- Backend supports register, login, JWT access-token generation, JWT bearer validation, and current-user lookup. References: `src/Api/Ecommerce.Api/Controllers/Auth/AuthUsersController.cs`, `src/Modules/Auth/Ecommerce.Auth.Application/Users`, `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Security`.
- Frontend supports REST-only login, register, current-user loading, session state, token storage abstraction, and protected route guards. References: `../zy-commerce-frontend/src/app/core/auth/auth-api.client.ts`, `../zy-commerce-frontend/src/app/core/auth/auth-session.service.ts`, `../zy-commerce-frontend/src/app/core/auth/auth.guard.ts`.

### Cart and Checkout

- Frontend cart stores product id, quantity, and display snapshots, persists cart state in session storage, and computes display subtotal. References: `../zy-commerce-frontend/src/app/features/cart/data/cart-state.service.ts`, `../zy-commerce-frontend/src/app/features/cart/data/cart.models.ts`.
- Frontend checkout builds backend-required order snapshot lines with `productId`, `quantity`, `productSku`, `productName`, and `unitPrice`. References: `../zy-commerce-frontend/src/app/features/orders/data/create-order.mapper.ts`, `../zy-commerce-frontend/docs/project/ADR-002-checkout-snapshot-fields.md`.
- Backend Orders create stores product snapshot data and calculates totals. References: `src/Modules/Orders/Ecommerce.Orders.Domain/Orders/Order.cs`, `src/Modules/Orders/Ecommerce.Orders.Domain/Orders/OrderLine.cs`, `docs/decisions/ADR-002-orders-product-snapshot-strategy.md`.

### Orders

- Backend supports protected order creation, listing current user order summaries, and getting one owner-scoped order. References: `src/Api/Ecommerce.Api/Controllers/Orders/OrdersController.cs`, `src/Modules/Orders/Ecommerce.Orders.Application/Orders`.
- Orders list is owner-scoped to JWT `sub`, paginated, and summary-only. References: `src/Modules/Orders/Ecommerce.Orders.Application/Orders/ListOrdersForBuyer/ListOrdersForBuyerQuery.cs`, `src/Modules/Orders/Ecommerce.Orders.Infrastructure/Orders/OrderReadRepository.cs`.
- Frontend supports Orders list, checkout, and order details routes and API client. References: `../zy-commerce-frontend/src/app/features/orders/orders.routes.ts`, `../zy-commerce-frontend/src/app/features/orders/data/orders-api.client.ts`.

### Health, Logging, Swagger, MCP

- Backend exposes `/health/live` and `/health/ready`. References: `src/Api/Ecommerce.Api/HealthChecks/DbContextHealthCheck.cs`, `src/Api/Ecommerce.Api/Program.cs`.
- Backend supports `X-Correlation-ID` request/response behavior and structured logging with token-safe constraints. References: `src/Api/Ecommerce.Api/Middleware/CorrelationIdMiddleware.cs`, `src/Api/Ecommerce.Api/Middleware/RequestLoggingMiddleware.cs`, `src/Api/Ecommerce.Api/Middleware/ExceptionHandlingMiddleware.cs`.
- Swagger/OpenAPI is available in local Development and marks protected operations. References: `src/Api/Ecommerce.Api/OpenApi/AuthorizationHeaderOperationFilter.cs`, `docs/project/PROJECT_STATUS.md`.
- Backend MCP endpoint `/mcp` is protected and exposes an allowlist of Catalog/Orders tools. References: `src/Api/Ecommerce.Api/Mcp/EcommerceMcpTools.cs`, `docs/decisions/ADR-003-mcp-server-boundary-and-security.md`.
- Backend assistant endpoint `POST /api/assistant/query` is protected and answers supported read-only ecommerce questions by composing approved Catalog and Orders read capabilities. References: `src/Api/Ecommerce.Api/Controllers/Assistant/AssistantController.cs`, `src/Api/Ecommerce.Api/Assistant`, `docs/decisions/ADR-004-assistant-orchestration-boundary-and-safety.md`.

## Partially Implemented

- Frontend MCP client infrastructure and assistant UI exist, but project docs say real MCP execution is not fully implemented. References: `../zy-commerce-frontend/src/app/mcp`, `../zy-commerce-frontend/src/app/features/mcp-assistant`, `../zy-commerce-frontend/docs/project/CURRENT_STATE.md`.
- Frontend product DTO models include optional fields such as `currencyCode` and `imageUrl`; backend Catalog currently documents SKU, name, description, price, active state, and timestamps. References: `../zy-commerce-frontend/src/app/features/catalog/data/catalog.models.ts`, `docs/project/FRONTEND_CONTRACT.md`.

## Skeleton Only

- Frontend MCP confirmation workflow is skeleton-first; mutating MCP tools require future confirmation UI. References: `../zy-commerce-frontend/src/app/mcp/mcp-confirmation.service.ts`, `../zy-commerce-frontend/docs/project/REST_MCP_BOUNDARIES.md`.

## Planned

- Product price update command/endpoints are roadmap candidates. Reference: `docs/project/ROADMAP.md`.
- Orders Catalog validation/integration is a candidate to reduce product snapshot spoofing risk. References: `docs/decisions/ADR-002-orders-product-snapshot-strategy.md`, `docs/project/ROADMAP.md`.
- Auth refresh tokens, broader auth policies, integration testing, API versioning, configuration validation, MCP policy/rate limiting, assistant frontend integration, and external LLM-backed assistant orchestration remain future candidates. References: `docs/project/ROADMAP.md`, `docs/project/NEXT_SESSION.md`.

## Intentionally Absent

- Backend intentionally lacks microservices, event bus, distributed transactions, startup auto-migrations, Docker setup, Customers, Inventory, payments, shipping, discounts/coupons, order cancellation/refunds, advanced order status workflow, Auth refresh tokens, roles/permissions, token persistence, and Catalog protected reads. Reference: `docs/project/PROJECT_STATUS.md`.
- Backend assistant intentionally lacks mutating actions, raw SQL access, cross-user access, admin analytics, and external LLM provider integration in Phase 1. Reference: `docs/decisions/ADR-004-assistant-orchestration-boundary-and-safety.md`.
- Frontend must not use MCP for login/register and must not store secrets in runtime config. References: `../zy-commerce-frontend/docs/project/REST_MCP_BOUNDARIES.md`, `../zy-commerce-frontend/docs/project/SECURITY.md`.

## Unknown / Not Verified

- Fresh backend build/test commands were run after Ecommerce Assistant Agent Phase 1, not during the earlier documentation-only pack. Frontend build/test commands were not run.
- Local database state was not verified.
- Runtime browser/API demos were not executed during this documentation-only task.
