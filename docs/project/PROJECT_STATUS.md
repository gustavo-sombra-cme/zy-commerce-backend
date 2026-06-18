# Project Status

## Snapshot

Date: 2026-06-18

The repository is a .NET 9 ASP.NET Core backend for an enterprise e-commerce system. It follows Clean Architecture First, Modular Monolith, Module Isolation, CQRS, and thin controller rules.

Agent operating rules are split between a short `AGENT.md` router and detailed files under `instructions/`.

**Project Memory Files:**

- `docs/project/PROJECT_STATUS.md` - Current implementation snapshot
- `docs/project/AI_HANDOFF.md` - Constraints and architecture for new sessions
- `docs/project/ROADMAP.md` - Completed and candidate work
- `docs/project/NEXT_SESSION.md` - Fast resume guide (< 5 minutes to read)
- `docs/project/PROMPT_TEMPLATE.md` - Reusable planning and execution prompt defaults for shorter future prompts
- `docs/project/FRONTEND_CONTRACT.md` - Frontend-facing API contract notes

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
- `src/Modules/Orders/Ecommerce.Orders.Domain`
- `src/Modules/Orders/Ecommerce.Orders.Application`
- `src/Modules/Orders/Ecommerce.Orders.Infrastructure`
- `src/Modules/Orders/Ecommerce.Orders.Contracts`
- `tests/ArchitectureTests/Ecommerce.ArchitectureTests`
- `tests/UnitTests/Ecommerce.Catalog.UnitTests`
- `tests/UnitTests/Ecommerce.Auth.UnitTests`
- `tests/UnitTests/Ecommerce.Orders.UnitTests`

## Active Modules

### Catalog

Catalog contains product business behavior.

Implemented product capabilities:

- Create Product
- Create Product with Catalog-owned price
- Get Product By Id
- Search/List Products with pagination
- Update Product Details
- Deactivate Product
- Reactivate Product

Catalog uses:

- DDD aggregate behavior for `Product`
- CQRS with MediatR
- FluentValidation
- API authorization on write endpoints
- EF Core persistence
- SQL Server LocalDB
- Manual EF Core migration: `InitialCatalogSchema`
- Manual EF Core migration: `AddProductPrice`
- Query-side read model for product search

### Auth

Auth has Register User, Login User, JWT access token generation, JWT bearer validation, and a protected Current User endpoint implemented through Domain, Application, Infrastructure, Contracts, and API. Login verifies credentials and returns user identity plus a short-lived JWT access token.

Current Auth projects:

- `Ecommerce.Auth.Domain`
- `Ecommerce.Auth.Application`
- `Ecommerce.Auth.Infrastructure`
- `Ecommerce.Auth.Contracts`
- `Ecommerce.Auth.UnitTests`

Implemented Auth domain model:

- `User` aggregate
- `UserId` value object
- `Email` value object
- `PasswordHash` value object

Implemented Auth domain behaviors:

- Register user
- Verify email
- Change password hash
- Deactivate user
- Reactivate user

Implemented Auth Application behavior:

- `RegisterUserCommand`
- `RegisterUserCommandHandler`
- `RegisterUserCommandValidator`
- `RegisterUserResult`
- `DuplicateEmailException`
- `IUserRepository` abstraction
- `IAuthUnitOfWork` abstraction
- `IPasswordHasher` abstraction
- `LoginUserCommand`
- `LoginUserCommandHandler`
- `LoginUserCommandValidator`
- `LoginUserResult`
- `InvalidCredentialsException`
- `InactiveUserException`
- `IUserRepository.GetByEmailAsync` abstraction
- `IPasswordHasher.Verify` abstraction
- `AccessTokenResult`
- `IAccessTokenGenerator`

Implemented Auth Infrastructure behavior:

- `AuthDbContext`
- `UserConfiguration`
- `UserRepository`
- `UserRepository.GetByEmailAsync`
- Infrastructure `PasswordHasher`
- `JwtOptions`
- `JwtAccessTokenGenerator`
- Auth Infrastructure dependency registration

Implemented Auth API behavior:

- `POST /api/auth/users/register`
- `RegisterUserRequest`
- `RegisterUserResponse`
- `201 Created` on successful registration
- `409 Conflict` on duplicate email
- `POST /api/auth/users/login`
- `LoginUserRequest`
- `LoginUserResponse`
- `200 OK` on successful login
- Login response includes `accessToken`, `tokenType`, and `expiresAt`
- `401 Unauthorized` on invalid credentials
- `403 Forbidden` on inactive user
- JWT bearer authentication middleware
- Explicit JWT `DefaultAuthenticateScheme` and `DefaultChallengeScheme`
- Authorization services and middleware
- `GET /api/auth/users/me`
- `GetCurrentUserResponse`
- `200 OK` with `userId` and `email` from JWT claims for valid bearer tokens
- `401 Unauthorized` for missing or invalid bearer tokens

Auth intentionally does not contain:

- refresh tokens
- roles or permissions
- token persistence

### Orders

Orders has Create Order, List Orders For Current User, and Get Order By Id implemented through Domain, Application, Infrastructure, Contracts, UnitTests, and API.

Current Orders projects:

- `Ecommerce.Orders.Domain`
- `Ecommerce.Orders.Application`
- `Ecommerce.Orders.Infrastructure`
- `Ecommerce.Orders.Contracts`
- `Ecommerce.Orders.UnitTests`

Implemented Orders domain model:

- `Order` aggregate
- `OrderLine` child entity
- `OrderId` value object
- `OrderLineId` value object
- `BuyerId` value object
- `OrderStatus`

Implemented Orders behavior:

- Create order
- Capture product snapshot data from the create request
- Calculate order totals from lines
- List order summaries for the authenticated buyer with pagination, newest first
- Get order by id for the authenticated owner only

Orders intentionally does not contain:

- payments
- inventory reservation
- shipping
- discounts
- coupons
- cancellation
- refunds
- advanced order status workflows
- Customer profile integration
- Catalog internal references
- Auth internal references
- MCP adapter code inside the Orders module

## API Status

Catalog endpoints are implemented through controllers under:

- `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`

Current product routes:

- `POST /api/catalog/products` - protected, requires valid bearer token
- `GET /api/catalog/products/{productId}`
- `GET /api/catalog/products`
- `PUT /api/catalog/products/{productId}` - protected, requires valid bearer token
- `DELETE /api/catalog/products/{productId}` - protected, requires valid bearer token
- `POST /api/catalog/products/{productId}/reactivate` - protected, requires valid bearer token

Current Auth routes:

- `POST /api/auth/users/register`
- `POST /api/auth/users/login`
- `GET /api/auth/users/me`

Current Orders routes:

- `POST /api/orders` - protected, requires valid bearer token
- `GET /api/orders` - protected, requires valid bearer token, returns only the authenticated user's order summaries, sorted newest first, with pagination defaults `pageNumber=1` and `pageSize=20`
- `GET /api/orders/{orderId}` - protected, requires valid bearer token and returns only the authenticated user's order

Current platform health routes:

- `GET /health/live` - process liveness only; does not depend on database connectivity
- `GET /health/ready` - readiness for Auth, Catalog, and Orders database connectivity

Health readiness checks use the existing Auth, Catalog, and Orders EF Core DbContexts with `Database.CanConnectAsync`. They do not create databases, apply migrations, or change schema.

Current platform logging behavior:

- Uses built-in ASP.NET Core logging only.
- Supports `X-Correlation-ID` on requests.
- Preserves incoming `X-Correlation-ID` values when supplied.
- Returns `X-Correlation-ID` on every response.
- Adds structured request, exception, health readiness failure, and JWT authentication failure/challenge logging.
- Does not log tokens, authorization headers, passwords, request bodies, or response bodies.

Current platform MCP behavior:

- Uses the official `ModelContextProtocol.AspNetCore` package in `Ecommerce.Api`.
- Hosts protected `POST /mcp` as a stateless Streamable HTTP MCP endpoint.
- Implements MCP as an API-layer adapter under `src/Api/Ecommerce.Api/Mcp`.
- MCP tools call existing Application/CQRS requests through `ISender`.
- MCP adapter code does not call EF Core DbContexts, repositories, Domain objects, or module internals directly.
- Approved initial MCP tool allowlist:
  - `catalog_search_products`
  - `catalog_get_product_by_id`
  - `orders_get_order_by_id`
  - `orders_create_order`
- `orders_create_order` requires explicit `confirmedByUser` input before dispatching the create command.
- Orders MCP reads are scoped to the authenticated user id from the JWT `sub` claim.
- MCP does not expose Auth register/login, JWTs, passwords, authorization headers, raw database access, migrations, health readiness details, appsettings, environment variables, SQL, Catalog writes, cross-user orders, or non-existent Orders features.

Swagger/OpenAPI is enabled for local Development only. Swagger uses the standard HTTP bearer security scheme and includes an Authorize button. Protected operations are marked with per-operation security metadata from `[Authorize]`, while public endpoints remain public. In Swagger Authorize, enter the raw JWT access token only; Swagger UI sends `Authorization: Bearer {token}`. Swagger UI persists authorization across browser refreshes.

## Database Status

Catalog has EF Core persistence and two approved migrations:

- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260608111338_InitialCatalogSchema.cs`
- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260618090000_AddProductPrice.cs`

Auth has EF Core persistence and one approved migration:

- `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Persistence/Migrations/20260609092505_InitialAuthSchema.cs`

Orders has EF Core persistence and one approved migration:

- `src/Modules/Orders/Ecommerce.Orders.Infrastructure/Persistence/Migrations/20260612090403_InitialOrdersSchema.cs`

Local development uses:

- `ConnectionStrings:Catalog`
- `ConnectionStrings:Auth`
- `ConnectionStrings:Orders`
- SQL Server LocalDB
- `(localdb)\mssqllocaldb`

The Auth LocalDB database `EcommerceAuth` has been updated through `InitialAuthSchema`.
The Orders LocalDB database `EcommerceOrders` has been created and updated through `InitialOrdersSchema`.

## Error Handling

The API has global exception middleware:

- Validation failures return `ValidationProblemDetails`.
- Duplicate SKU returns conflict.
- Duplicate email returns conflict.
- Invalid credentials return unauthorized.
- Inactive users return forbidden.
- Not found support returns not found where used.
- Unknown exceptions return generic server errors.

Controllers keep transport-level checks, such as explicit `Guid.Empty` handling.

## Architecture Rules

Architecture tests enforce:

- Domain projects do not reference Application, Infrastructure, Contracts, or Api.
- Application projects do not reference Infrastructure or Api.
- Infrastructure projects do not reference Api.
- BuildingBlocks projects do not reference Catalog, Auth, or Orders.
- Catalog, Auth, and Orders do not reference each other.
- Catalog write endpoints require authorization.
- Catalog read endpoints remain public.
- Auth register and login remain public.
- Auth current-user endpoint remains protected.
- Orders endpoints require authorization.
- MCP `/mcp` endpoint requires authorization.
- MCP exposes only the approved tool allowlist.
- MCP adapter types must not depend on EF Core, repositories, Domain projects, or module persistence internals.
- Only approved projects exist.
- Only approved modules exist.
- No Bootstrapper or Shared projects exist.

Agent rule files:

- `AGENT.md` is the short router.
- `instructions/00-role-and-stack.md` contains role, stack, strategy, repository, and active module rules.
- `instructions/01-execution-and-planning.md` contains state, execution lock, planning rules, and short-prompt expansion rules.
- `instructions/02-architecture-and-modules.md` contains Clean Architecture and module rules.
- `instructions/03-cqrs-database-testing-security.md` contains CQRS, DDD ownership, database, testing, architecture test, and security rules.
- `instructions/04-documentation-and-memory.md` contains prompt logging, reusable template location, project memory, AGENT replacement, ADR, and learning journal rules.
- `instructions/05-completion.md` contains self-review, execution summary, and completion rules.
- `docs/project/PROMPT_TEMPLATE.md` defines default PLAN MODE and APPROVED EXECUTE expectations, including a strict Plan Output Contract and Plan Self-Validation Rule so future prompts can be shorter without weakening execution lock, prompt logging, architecture, DDD, CQRS, module isolation, testing, or documentation rules.

## Test Status

Latest known code execution after Revisit Swagger/API Authentication Integration:

- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 61 passed
- Architecture tests: 24 passed
- Auth unit tests: 65 passed
- Local Swagger JSON verification confirmed the bearer scheme, per-operation security metadata, and documented `204`/`401` DELETE responses.
- Runtime verification confirmed protected DELETE returns `401` without a token and `204` with `Authorization: Bearer {token}`.
- No package, migration, project, JWT runtime, Auth behavior, or Catalog behavior changes were created.

Latest documentation-only execution after Prompt Standardization And Reusable Template Setup:

- No code or project structure changed.
- Restore, build, and test were intentionally not run.
- Documentation self-review was performed.

Latest documentation-only execution after Prompt Template Compliance Contract Enhancement:

- `docs/project/PROMPT_TEMPLATE.md` now requires exact planning section names, a full plan output structure, and internal self-validation before returning plans from short prompts.
- `instructions/01-execution-and-planning.md` now points short planning prompts to the full template contract.
- No code or project structure changed.
- Restore, build, and test were intentionally not run.
- Documentation self-review was performed.

Latest code execution after Catalog Reactivate Product:

- `Product.Reactivate` implements an idempotent inactive-to-active lifecycle transition.
- Reactivating an already active product returns success without changing state or updating `UpdatedAt`.
- `POST /api/catalog/products/{productId}/reactivate` is protected and returns `204 No Content` on success.
- No package, migration, schema, Auth behavior, or cross-module changes were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 70 passed
- Architecture tests: 26 passed
- Auth unit tests: 65 passed
- Manual API smoke verification was attempted but could not complete because the local SQL Server LocalDB runtime was unavailable in this environment.

Latest code execution after Platform Health Checks:

- `GET /health/live` was added for process liveness only.
- `GET /health/ready` was added for Auth and Catalog database readiness.
- Readiness uses EF Core `Database.CanConnectAsync` and does not create databases, apply migrations, or change schema.
- No package, migration, schema, Domain, Application, CQRS, Auth behavior, Catalog behavior, or module behavior changes were created.
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln --no-build`: passed
- Catalog unit tests: 70 passed
- Architecture tests: 26 passed
- Auth unit tests: 65 passed

Latest documentation-only execution after Health Checks Documentation Finalization:

- Prompt logs and project memory docs were updated for Platform Health Checks.
- No code or project structure changed.
- Build and test were intentionally not run.

Latest code execution after Platform Structured Logging:

- Added `X-Correlation-ID` support and returns the header on every response.
- Preserves incoming `X-Correlation-ID` values when supplied.
- Added built-in ASP.NET Core structured request logging.
- Added structured exception logging without logging handled exception details that may contain user input.
- Added health readiness failure logging.
- Added JWT authentication failure and challenge logging without logging tokens or authorization headers.
- Does not log tokens, authorization headers, passwords, request bodies, or response bodies.
- No package, migration, schema, Domain, Application, CQRS, module, or business behavior changes were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 70 passed
- Architecture tests: 30 passed
- Auth unit tests: 65 passed

Latest code execution after Orders Initial Vertical Slice:

- Added the Orders module with Domain, Application, Infrastructure, Contracts, and UnitTests projects.
- Added `Order` aggregate, `OrderLine`, `OrderId`, `OrderLineId`, `BuyerId`, and `OrderStatus`.
- Added Create Order and Get Order By Id CQRS flows.
- Create Order captures product snapshot data from the request.
- Get Order By Id is scoped to the authenticated buyer id.
- Added protected `POST /api/orders` and `GET /api/orders/{orderId}` endpoints.
- Added ADR-002 for Orders product snapshot strategy.
- Added initial Orders EF Core migration `InitialOrdersSchema`.
- Created and updated the local `EcommerceOrders` database through `InitialOrdersSchema`.
- No Catalog or Auth internal references were added.
- No payments, inventory, shipping, discounts, coupons, refunds, cancellation, advanced order workflows, Customer profile module, or MCP integration were added.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 70 passed
- Auth unit tests: 65 passed
- Orders unit tests: 12 passed
- Architecture tests: 33 passed
- Manual API smoke verification passed for unauthorized create, authenticated create, same-user get, cross-user get returning `404 NotFound`, and correlation header preservation.

Latest code execution after MCP Server Integration:

- Added the official `ModelContextProtocol.AspNetCore` package to `Ecommerce.Api`.
- Added a protected `/mcp` stateless Streamable HTTP endpoint.
- Added API-layer MCP tools under `src/Api/Ecommerce.Api/Mcp`.
- MCP tools dispatch existing Catalog and Orders CQRS requests through `ISender`.
- Added approved MCP tools only: `catalog_search_products`, `catalog_get_product_by_id`, `orders_get_order_by_id`, and `orders_create_order`.
- `orders_create_order` requires explicit `confirmedByUser` input and has test coverage.
- Added ADR-003 for MCP boundary and security.
- No Domain, Application, Infrastructure module, CQRS, database schema, migration, Auth behavior, Catalog behavior, or Orders behavior changes were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 70 passed
- Auth unit tests: 65 passed
- Orders unit tests: 12 passed
- Architecture tests: 43 passed

Latest code execution after Orders List For Current User:

- Added query-side CQRS list flow for authenticated buyer order summaries.
- Added protected `GET /api/orders` endpoint with `pageNumber` and `pageSize` query parameters.
- Defaults are `pageNumber=1` and `pageSize=20`; `pageSize` is limited to `100`.
- List results are scoped to the JWT `sub` claim, sorted by `CreatedAt` descending, and return summary fields only: `orderId`, `status`, `totalAmount`, `createdAt`, and `lineCount`.
- Full order lines remain available only through `GET /api/orders/{orderId}`.
- Added frontend contract documentation in `docs/project/FRONTEND_CONTRACT.md`.
- No migrations, schema changes, commands, domain behavior, MCP changes, packages, Auth changes, or Catalog changes were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln --no-restore`: passed
- `dotnet test Ecommerce.sln --no-build`: passed
- Catalog unit tests: 70 passed
- Auth unit tests: 65 passed
- Orders unit tests: 23 passed
- Architecture tests: 48 passed

Latest code execution after Catalog Product Price Write Support:

- Added `Price` to the Catalog `Product` aggregate.
- `POST /api/catalog/products` now accepts non-negative `price` and stores it with `decimal(18,2)` precision.
- `GET /api/catalog/products` and `GET /api/catalog/products/{productId}` return `price`.
- Added Catalog migration `AddProductPrice`, defaulting existing rows to `0.00`.
- Updated Catalog tests and frontend contract documentation.
- No price update endpoint, price history/audit, currency support, discounts, coupons, MCP changes, Auth changes, Orders behavior changes, packages, or new modules were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 75 passed
- Auth unit tests: 65 passed
- Orders unit tests: 23 passed
- Architecture tests: 48 passed

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
- Inventory module
- payments, inventory reservation, shipping, discounts, coupons, order cancellation, refunds, advanced order status workflows, and Customer profile integration
- Auth refresh token, roles, permissions, protected Catalog read endpoint, or token persistence features
- MCP tools beyond the approved initial allowlist
