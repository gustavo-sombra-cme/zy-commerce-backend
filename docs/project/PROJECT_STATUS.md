# Project Status

## Snapshot

Date: 2026-06-25

The repository is a .NET 9 ASP.NET Core backend for an enterprise e-commerce system. It follows Clean Architecture First, Modular Monolith, Module Isolation, CQRS, and thin controller rules.

Agent operating rules are split between a short `AGENT.md` router and detailed files under `instructions/`.

**Project Memory Files:**

- `docs/project/PROJECT_STATUS.md` - Current implementation snapshot
- `docs/project/AI_HANDOFF.md` - Constraints and architecture for new sessions
- `docs/project/ROADMAP.md` - Completed and candidate work
- `docs/project/NEXT_SESSION.md` - Fast resume guide (< 5 minutes to read)
- `docs/project/PROMPT_TEMPLATE.md` - Reusable planning and execution prompt defaults for shorter future prompts
- `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md` - Repo-local AI workflow skill and sub-agent architecture decisions
- `docs/project/CODE_REVIEW.md` - Backend code review checklist for code/config/migration/CI/runtime-behavior documentation changes
- `docs/project/FRONTEND_CONTRACT.md` - Frontend-facing API contract notes
- `docs/skills/workflow/` - Repo-local workflow skill docs for repeated Codex/harness checks
- `docs/agents/workflow/` - Repo-local workflow sub-agent responsibility guidance
- `docs/demo/features/` - Feature-focused demo slide source files for main/demo-worthy feature executions

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
- Update Product Price
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

Auth has Register User, Login User, JWT access token generation, JWT bearer validation, Admin/Customer role support, and a protected Current User endpoint implemented through Domain, Application, Infrastructure, Contracts, and API. Login verifies credentials and returns user identity plus a short-lived JWT access token that includes a `role` claim.

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
- `UserRole` enum

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
- JWT role claim generation using `role`
- `RequireAdmin` authorization policy in the API
- Explicit JWT `DefaultAuthenticateScheme` and `DefaultChallengeScheme`
- Authorization services and middleware
- `GET /api/auth/users/me`
- `GetCurrentUserResponse`
- `200 OK` with `userId`, `email`, and `role` from JWT claims for valid bearer tokens
- `401 Unauthorized` for missing or invalid bearer tokens

Auth intentionally does not contain:

- refresh tokens
- token persistence
- public admin registration endpoint

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

- `POST /api/catalog/products` - admin-protected, requires valid bearer token with `Admin` role
- `GET /api/catalog/products/{productId}`
- `GET /api/catalog/products`
- `PUT /api/catalog/products/{productId}` - admin-protected, requires valid bearer token with `Admin` role
- `PUT /api/catalog/products/{productId}/price` - admin-protected, requires valid bearer token with `Admin` role
- `DELETE /api/catalog/products/{productId}` - admin-protected, requires valid bearer token with `Admin` role
- `POST /api/catalog/products/{productId}/reactivate` - admin-protected, requires valid bearer token with `Admin` role

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

Current platform assistant behavior:

- Hosts protected `POST /api/assistant/query`.
- Keeps the `POST /api/assistant/query` request/response contract unchanged.
- Uses `IAssistantIntentInterpreter` with `DeterministicAssistantIntentInterpreter` as the disabled-mode production default.
- Can use `LlmAssistantIntentInterpreter` when `Assistant:Llm:Enabled` is explicitly enabled and provider/model/API key configuration is supplied outside committed secrets.
- Provider selection defaults to the existing OpenAI-style `HttpAssistantLlmClient`; `ECOMMERCE_ASSISTANT_LLM_PROVIDER=Gemini` selects `GeminiAssistantLlmClient`.
- Gemini is supported as a POC/testing provider through `ECOMMERCE_ASSISTANT_GEMINI_API_KEY`, optional `ECOMMERCE_ASSISTANT_GEMINI_MODEL` (default `gemini-2.5-flash`), and optional `ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT` (default `https://generativelanguage.googleapis.com/v1beta`).
- Gemini free-tier and rate-limit behavior depends on Google account/project; a ChatGPT/OpenAI subscription is unrelated to Gemini Developer API access.
- The LLM provider adapters use `HttpClientFactory` and `System.Text.Json`; no provider SDK package was added.
- Treats interpreter output as an untrusted `AssistantIntentPlan` that must pass `AssistantIntentPlanValidator` before execution.
- Validates proposed intent kind, tool names, and arguments against the approved read-only assistant capability allowlist.
- Rejects unknown tools, unsafe questions, mutating/admin/SQL/cross-user plans, and model-provided `userId`/`buyerId` scope.
- No API key, secret, provider SDK package, live test call, runtime database access, or MCP dependency has been added for LLM provider execution.
- Assistant Text-to-SQL Task 1 added only the future database boundary: approved read-only views under the `assistant` schema and setup documentation in `docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md`.
- Assistant Text-to-SQL Task 2 added a SQL validator and read-only executor behind `Assistant:TextToSql:Enabled`, which defaults to `false`.
- Assistant Text-to-SQL Task 3 added an LLM planner that builds the approved-view Text-to-SQL prompt, parses the model JSON plan fail-closed, and reuses the existing assistant LLM client abstraction.
- Assistant Text-to-SQL Task 4 wired `AssistantOrchestrator` to use Text-to-SQL as an optional first-pass path only when `Assistant:TextToSql:Enabled` is true.
- Existing assistant behavior is unchanged when the feature flag is disabled, and existing assistant behavior remains fallback when Text-to-SQL fails safely.
- Text-to-SQL candidate SQL is still untrusted and must pass the Task 2 validator before execution.
- Generated SQL is not returned to the frontend, and `genericTable` is not exposed publicly.
- Assistant Text-to-SQL Task 5A cleaned stale documentation/status/test naming only. Deterministic fallback, the existing CQRS assistant flow, response DTOs, tool names, and feature-flag behavior remain in place.
- Future Text-to-SQL runtime must use separate local-only read-only connection strings: `ConnectionStrings:AssistantCatalogReadOnly` and `ConnectionStrings:AssistantOrdersReadOnly`.
- Text-to-SQL execution must not use normal application DB connection strings.
- No prompts, raw provider responses, API keys, JWTs, auth headers, full Gemini request URIs, or sensitive payloads are logged.
- Assistant implementation lives under `src/Api/Ecommerce.Api/Assistant` with transport in `src/Api/Ecommerce.Api/Controllers/Assistant`.
- Assistant capabilities are internally allowlisted as:
  - `catalog_search`
  - `catalog_get_product`
  - `orders_search`
  - `orders_get_order`
  - `orders_analyze`
- Assistant dispatches existing Catalog and Orders read-side CQRS queries through `ISender`.
- Assistant does not call EF Core DbContexts, repositories, Domain objects, or module internals directly.
- Orders analysis uses the authenticated JWT `sub` claim as buyer scope and does not accept buyer/user id from the request body.
- Supported Phase 1 questions include recent orders, products ordered, orders containing a product/SKU/name, total spend, most frequently purchased products, products under an amount, and orders containing products over an amount.
- Mutating, admin, SQL, token, database, internal implementation, unclear, and cross-user questions return a safe unsupported response.
- ADR-004 documents the assistant orchestration boundary and safety model.
- ADR-005 documents untrusted assistant intent interpretation and plan validation.
- ADR-006 documents the config-gated provider-backed LLM interpreter.

Swagger/OpenAPI is enabled for local Development only. Swagger uses the standard HTTP bearer security scheme and includes an Authorize button. Protected operations are marked with per-operation security metadata from `[Authorize]`, while public endpoints remain public. In Swagger Authorize, enter the raw JWT access token only; Swagger UI sends `Authorization: Bearer {token}`. Swagger UI persists authorization across browser refreshes.

## Database Status

Catalog has EF Core persistence and two approved migrations:

- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260608111338_InitialCatalogSchema.cs`
- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260618090000_AddProductPrice.cs`
- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260624090000_AddAssistantCatalogReadOnlyViews.cs`

Auth has EF Core persistence and one approved migration:

- `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Persistence/Migrations/20260609092505_InitialAuthSchema.cs`
- `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Persistence/Migrations/20260623090000_AddUserRole.cs`

Orders has EF Core persistence and approved migrations:

- `src/Modules/Orders/Ecommerce.Orders.Infrastructure/Persistence/Migrations/20260612090403_InitialOrdersSchema.cs`
- `src/Modules/Orders/Ecommerce.Orders.Infrastructure/Persistence/Migrations/20260624090000_AddAssistantReadOnlyViews.cs`

Assistant read-only views are created under the `assistant` schema:

Catalog database:

- `assistant.v_ProductSearch`
- `assistant.v_ProductDetails`

Orders database:

- `assistant.v_MyOrders`
- `assistant.v_MyOrderLines`
- `assistant.v_MyOrderSummary`

Catalog and Orders use separate physical databases. Assistant views are created separately in their owning databases; Task 1 does not use cross-database views, linked servers, synonyms, or a combined Catalog/Orders database assumption.

Local development uses:

- `ConnectionStrings:Catalog`
- `ConnectionStrings:Auth`
- `ConnectionStrings:Orders`
- SQL Server LocalDB
- `(localdb)\mssqllocaldb`

The Auth LocalDB database `EcommerceAuth` has been updated through `InitialAuthSchema`.
The Orders LocalDB database `EcommerceOrders` has been created and updated through `InitialOrdersSchema`.
Assistant read-only DB users must be created manually in the Catalog and Orders databases and granted `SELECT` only on the local `assistant` schema/views. Do not commit real `ConnectionStrings:AssistantCatalogReadOnly` or `ConnectionStrings:AssistantOrdersReadOnly` values or passwords.

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
- Assistant `POST /api/assistant/query` endpoint requires authorization.
- Assistant exposes only the approved read-only capability allowlist.
- Assistant API-layer types must not depend on EF Core, repositories, Domain projects, module persistence internals, or MCP protocol packages.
- Assistant source must not reference write commands.
- MCP exposes only the approved tool allowlist.
- MCP adapter types must not depend on EF Core, repositories, Domain projects, or module persistence internals.
- Only approved projects exist.
- Only approved modules exist.
- No Bootstrapper or Shared projects exist.

Agent rule files:

- `AGENT.md` is the short router.
- `instructions/00-role-and-stack.md` contains role, stack, strategy, repository, and active module rules.
- `instructions/01-execution-and-planning.md` contains state, execution lock, planning rules, and short-prompt expansion rules.
- `instructions/01-execution-and-planning.md` also contains backend branch workflow rules requiring one task, one branch, one PR; latest `main` start; dirty-worktree safety; and manual commit/push/PR gates.
- `instructions/02-architecture-and-modules.md` contains Clean Architecture and module rules.
- `instructions/03-cqrs-database-testing-security.md` contains CQRS, DDD ownership, database, testing, architecture test, and security rules.
- `instructions/04-documentation-and-memory.md` contains prompt logging, reusable template location, project memory, repo-local workflow skills, feature demo slide deliverables, AGENT replacement, ADR, and learning journal rules.
- `instructions/05-completion.md` contains self-review, code review, execution summary, and completion rules.
- `docs/project/PROMPT_TEMPLATE.md` defines default PLAN MODE and APPROVED EXECUTE expectations, including a strict Plan Output Contract, Plan Self-Validation Rule, branch workflow summary, repo-local workflow skill references, and feature demo slide deliverable rule so future prompts can be shorter without weakening execution lock, prompt logging, architecture, DDD, CQRS, module isolation, testing, or documentation rules.
- Approved backend execution tasks must create a dedicated `feature/`, `fix/`, `docs/`, or `chore/` branch from latest `main` before implementation and must not commit, push, or create a PR without explicit backend approval.
- Repo-local workflow skills live under `docs/skills/workflow/` and are Markdown guidance only. They are not approval by themselves and are not shared with the frontend repo.
- Phase 2A wiring maps repo-local workflow skills and workflow sub-agent guidance into `AGENT.md`, `instructions/*`, and `docs/project/PROMPT_TEMPLATE.md`. This is workflow/documentation wiring only and does not change runtime behavior.
- Runtime assistant sub-agents, if introduced later, must remain API-layer classes and dispatch business reads through existing Application/CQRS handlers.

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

Latest documentation-only execution after Feature Demo Slide Deliverable Workflow:

- `docs/project/PROMPT_TEMPLATE.md` now requires feature demo slide Markdown deliverables for main features, major platform capabilities, API modules, integrations, and demo-worthy backend behavior.
- `instructions/04-documentation-and-memory.md` now defines the durable feature demo slide deliverable rule.
- Default slide source location is `docs/demo/features/{feature-slug}-demo-slides.md`.
- `docs/demo/features/` was added as the standard location for future feature demo slide sources.
- Tiny fixes, typo fixes, internal refactors with no demo value, prompt-template-only cleanup, test-only cleanup, and documentation-only maintenance do not require slide files unless explicitly requested.
- No application code, project references, packages, APIs, database schema, or migrations changed.
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

Latest code execution after Ecommerce Assistant Agent Phase 1:

- Added protected `POST /api/assistant/query`.
- Added deterministic API-layer assistant orchestration with an internal read-only Catalog/Orders capability allowlist.
- Added safe unsupported handling for mutating, admin, SQL, token, database, internal, unclear, and cross-user requests.
- Orders assistant analysis is owner-scoped to the authenticated JWT `sub` claim.
- Assistant dispatches existing read-side CQRS queries through `ISender` and does not call EF Core DbContexts, repositories, Domain objects, module internals, or MCP protocol code directly.
- Added ADR-004 for the assistant orchestration boundary and safety model.
- No packages, migrations, database changes, external AI provider, MCP changes, Catalog writes, Orders writes, Auth behavior changes, or frontend behavior changes were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: blocked by running process `Ecommerce.Api (36008)` locking `src/Api/Ecommerce.Api/bin/Debug/net9.0/Ecommerce.Api.dll`
- `dotnet build Ecommerce.sln --artifacts-path artifacts\assistant-verify`: passed
- `dotnet test Ecommerce.sln --artifacts-path artifacts\assistant-test`: passed
- Catalog unit tests: 75 passed
- Auth unit tests: 65 passed
- Orders unit tests: 23 passed
- Architecture tests: 71 passed

Latest code execution after Assistant Intent Interpreter Phase 2:

- Added `IAssistantIntentInterpreter` and `DeterministicAssistantIntentInterpreter`.
- Added `AssistantIntentPlan`, `AssistantSafetyPolicy`, and `AssistantIntentPlanValidator`.
- Kept deterministic interpretation as the production default and fallback path.
- Added fake interpreter support through tests only; no real external LLM provider, provider package, API key, secret, configuration value, or live network call was added.
- Kept `POST /api/assistant/query` contract unchanged.
- Kept assistant execution read-only, owner-scoped to JWT `sub`, and routed through existing Catalog/Orders read-side CQRS queries via `ISender`.
- Invalid plans, unknown tools, unsafe questions, and model-provided user/buyer scope fail closed without dispatch.
- Added ADR-005 for untrusted assistant intent interpretation.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 75 passed
- Auth unit tests: 65 passed
- Orders unit tests: 23 passed
- Architecture tests: 81 passed

Latest code execution after Assistant LLM Provider Integration Phase 3:

- Added config-gated `LlmAssistantIntentInterpreter`.
- Added `AssistantLlmOptions`, `IAssistantLlmClient`, `HttpAssistantLlmClient`, and `AssistantIntentPlanJsonParser`.
- Added non-secret `Assistant:Llm` settings in `appsettings.json`; API keys remain external.
- The API key is resolved at runtime from the configured environment variable name or non-committed user-secrets/config binding.
- LLM output is parsed only as structured `AssistantIntentPlan` JSON and still validated by `AssistantIntentPlanValidator` before execution.
- Deterministic interpretation remains disabled-mode default and fallback for missing secret, disabled config, provider failure, timeout/cancellation, and malformed JSON.
- Added fake provider/client tests only; no live provider calls were added.
- Added ADR-006 for provider-backed LLM interpretation.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 75 passed
- Auth unit tests: 65 passed
- Orders unit tests: 23 passed
- Architecture tests: 88 passed

Latest code execution after Assistant LLM Configuration Diagnostics:

- Added temporary safe diagnostic logs for assistant LLM configuration and fallback behavior.
- Logs include only booleans/presence flags for LLM enabled state, endpoint presence/validity, model presence, API key environment variable name presence, API key resolved state, provider call attempted/failed state, deterministic fallback usage, and model output validation failure.
- No API key values, prompts, raw model responses, auth headers, tokens, or sensitive payloads are logged.
- Assistant behavior was not changed.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 75 passed
- Auth unit tests: 65 passed
- Orders unit tests: 23 passed
- Architecture tests: 88 passed

Latest code execution after Backend Admin Product Management:

- Added Auth-owned `UserRole` support with default `Customer` registration and persisted `Users.Role`.
- Added Auth migration `AddUserRole`.
- JWT access tokens now include a `role` claim and API authorization uses `RoleClaimType = "role"`.
- `GET /api/auth/users/me` now returns `role`.
- Added API `RequireAdmin` policy.
- Catalog write endpoints now require Admin role; Catalog read endpoints remain public.
- Added `PUT /api/catalog/products/{productId}/price` for Admin-only price updates.
- Added Catalog `Product.UpdatePrice`, CQRS command/handler/validator, and contract request.
- Documented safe local Admin promotion through development database update only; no public admin registration endpoint or committed credentials were added.
- MCP and Assistant allowlists were not expanded with admin tools.
- Added `docs/demo/features/admin-product-management-demo-slides.md`.
- Verification results are recorded in the task execution summary.

Latest code execution after Backend Gemini LLM Provider:

- Added Gemini as a selectable external intent-interpretation provider for the existing read-only Ecommerce Assistant.
- Added `GeminiAssistantLlmClient` using REST, `HttpClientFactory`, and `System.Text.Json`.
- Added provider selection so OpenAI/default configuration keeps using `HttpAssistantLlmClient`, while `ECOMMERCE_ASSISTANT_LLM_PROVIDER=Gemini` selects Gemini.
- Gemini request handling sends only the read-only intent-planning instruction and user question to `generateContent`; it does not send catalog data, order history, secrets, JWTs, or auth headers.
- Gemini response handling extracts `candidates[].content.parts[].text` and returns it to the existing parser and validator.
- Deterministic fallback remains available for disabled configuration, missing API key/config, provider failure, rate limit, timeout, malformed provider JSON, missing candidate text, and invalid `AssistantIntentPlan` JSON.
- Assistant remains read-only and does not expose admin tools or write tools.
- No frontend, MCP, database, migration, Domain, Application, Infrastructure, package, assistant response contract, or committed secret changes were created.
- `dotnet restore Ecommerce.sln`: passed
- `dotnet build Ecommerce.sln`: passed
- `dotnet test Ecommerce.sln`: passed
- Catalog unit tests: 83 passed
- Auth unit tests: 68 passed
- Orders unit tests: 23 passed
- Architecture tests: 101 passed

Latest code execution after Assistant Text-to-SQL LLM Planner:

- Added a dormant Text-to-SQL planner under `src/Api/Ecommerce.Api/Assistant/TextToSql`.
- Added approved-view prompt generation for Catalog and Orders assistant views using only actual view columns.
- Added fail-closed JSON plan parsing for `supported`, `dataSource`, `sql`, `resultShape`, and `reason`.
- Reused the existing assistant LLM client abstraction; no provider SDK package, committed secret, live provider test, database migration, frontend, MCP, or assistant orchestration wiring was added.
- Candidate SQL remains untrusted and must pass the Task 2 validator before any future execution.
- Existing `POST /api/assistant/query` behavior remains unchanged.

Latest code execution after Assistant Text-to-SQL Orchestration:

- Wired `AssistantOrchestrator` to try Text-to-SQL first only when `Assistant:TextToSql:Enabled` is true.
- Added `AssistantTextToSqlResponseMapper` for existing frontend-compatible assistant response shapes.
- Preserved the existing assistant flow when Text-to-SQL is disabled and as fallback for planner unsupported output, validation failure, executor failure, unmappable shapes, and safe exceptions.
- Orders Text-to-SQL execution passes the authenticated backend buyer id as `@CurrentUserId`; catalog execution remains public catalog scope.
- Generated SQL, raw SQL errors, prompts, provider responses, secrets, JWTs, and connection strings are not returned to the frontend.
- No frontend, MCP, database migration, schema, admin tool, write behavior, provider SDK, or committed secret changes were added.

Latest code execution after Conservative Assistant Text-to-SQL Status Cleanup:

- Updated project documentation to describe Text-to-SQL as feature-flagged and wired rather than fully dormant.
- Clarified that the existing CQRS assistant flow remains the fallback path when Text-to-SQL is disabled or fails safely.
- Clarified that Task 5A did not remove deterministic fallback, the existing CQRS assistant flow, response DTOs, tool names, or feature-flag behavior.
- Renamed stale Text-to-SQL registration test wording from dormant to feature-flagged.
- No runtime behavior, frontend, MCP, database schema, migration, response contract, tool name, or secret changes were added.

Latest documentation-only execution after Backend Branch Workflow Rules:

- Added backend branch workflow rules to the execution/planning instructions.
- Backend execution now requires one task, one branch, one PR; latest `main` start; dirty-worktree safety; and manual commit, push, and PR gates.
- Updated reusable prompt defaults and project memory to preserve the rule for future sessions.
- No application code, project references, packages, APIs, database schema, or migrations changed.
- Restore, build, and test were intentionally not run.

Latest documentation-only execution after AI Skills And Sub-Agent Architecture:

- Added `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md` to document repo-local workflow skills, workflow sub-agent guidance, API-layer runtime sub-agent decisions, and Text-to-SQL non-migration.
- Added `docs/project/CODE_REVIEW.md`.
- Added repo-local workflow skill docs under `docs/skills/workflow/`.
- Added workflow sub-agent guidance under `docs/agents/workflow/`.
- Updated AGENT router, instructions, prompt template, and project memory to reference the new docs without making push automatic.
- No runtime assistant code, Text-to-SQL implementation, MCP code, frontend files, migrations, database schema, appsettings secrets, or response contracts changed.
- Restore, build, and test were intentionally not run because this was documentation/workflow only.

Latest documentation-only execution after Phase 2A Workflow Skill Wiring:

- Wired repo-local workflow skill docs and workflow sub-agent docs into `AGENT.md`, execution/planning instructions, documentation/memory instructions, completion instructions, and `docs/project/PROMPT_TEMPLATE.md`.
- Improved `docs/project/CODE_REVIEW.md`, workflow skill docs, and workflow sub-agent docs with clearer required/optional usage, examples, output formats, stop conditions, and AI/Text-to-SQL safety checks.
- Updated project memory and added prompt log `docs/prompts/091-phase-2-workflow-skill-wiring-execution.md`.
- This was workflow/documentation wiring only. No runtime assistant code, Text-to-SQL implementation, MCP code, frontend files, migrations, database schema, appsettings secrets, CI, project files, or response contracts changed.
- Restore, build, and test were intentionally not run because this was documentation/instruction only.

Latest code execution after Phase 3A Runtime Orders Assistant Sub-Agent:

- Extracted order-specific assistant CQRS orchestration into API-layer `IOrdersAssistantSubAgent` and `OrdersAssistantSubAgent`.
- `AssistantOrchestrator` remains the high-level coordinator for Text-to-SQL first-pass/fallback, intent interpretation, catalog handling, unsupported responses, and response contracts.
- Text-to-SQL remains as-is and was not converted into a skill or selectable runtime strategy in this task.
- Added assistant test configuration isolation so local assistant environment variables do not change architecture test expectations.
- Added safe non-secret disabled `Assistant:TextToSql` defaults to `appsettings.json`; no read-only connection strings or secrets were committed.
- No frontend, MCP, database schema, migration, provider SDK, project file, CI, or response contract changes were added.
- `dotnet restore Ecommerce.sln`: passed with NU1900 warnings for unreachable vulnerability feeds.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\phase3a-build`: passed.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\phase3a-test`: passed.
- Catalog unit tests: 83 passed.
- Auth unit tests: 68 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 165 passed.

Latest code execution after Phase 3B Runtime Catalog Assistant Sub-Agent:

- Extracted catalog-specific assistant CQRS orchestration into API-layer `ICatalogAssistantSubAgent` and `CatalogAssistantSubAgent`.
- `AssistantOrchestrator` remains the high-level coordinator for Text-to-SQL first-pass/fallback, intent interpretation, order delegation, catalog delegation, unsupported responses, and response contracts.
- Catalog assistant behavior is intended to remain unchanged for `CatalogProductsUnderPrice` and `CatalogGetProduct`.
- Text-to-SQL remains unchanged.
- `OrdersAssistantSubAgent` remains unchanged.
- Broad product search by text/name/SKU was not added in this phase.
- No frontend, MCP, database schema, migration, provider SDK, CI, appsettings secret, tool allowlist, genericTable exposure, admin/write assistant action, or response contract changes were added.
- `dotnet restore Ecommerce.sln`: passed with NU1900 warnings for unreachable vulnerability feeds.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\phase3b-build`: passed.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\phase3b-test`: passed.
- Catalog unit tests: 83 passed.
- Auth unit tests: 68 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 167 passed.

Latest documentation-only execution after Phase 3C Assistant Orchestrator Cleanup Review:

- Reviewed `AssistantOrchestrator` after Phase 3A Orders and Phase 3B Catalog runtime API-layer sub-agent extraction.
- Confirmed `AssistantOrchestrator` is now coordination-focused: top-level query flow, Text-to-SQL first-pass/fallback, intent interpretation, validation, diagnostics, sub-agent delegation, and final unsupported fallback.
- Confirmed Orders-specific CQRS assistant orchestration lives in `OrdersAssistantSubAgent`.
- Confirmed Catalog-specific CQRS assistant orchestration lives in `CatalogAssistantSubAgent`.
- Recommended not adding `SupportAssistantSubAgent` or `SafetyAssistantSubAgent` now.
- Text-to-SQL remains unchanged inside `AssistantOrchestrator`.
- Future work should focus on real assistant capabilities, not premature abstraction; future selectable Text-to-SQL strategy/telemetry should be planned separately if needed.
- No runtime behavior, Text-to-SQL code, frontend, MCP, database schema, migration, appsettings secret, tool allowlist, `AssistantQueryResponse`, `genericTable`, admin/write assistant action, or refusal wording changes were added.
- Restore, build, and test were intentionally not run because this was documentation-only.

Latest code execution after Assistant Broad Catalog Search:

- Added read-only assistant broad catalog search for natural product discovery questions such as "show me Galaxy products", "find iPhone products", "search for SKU ABC123", "do you have headphones", and "show active products matching laptop".
- Added `CatalogSearchProducts` as an assistant intent that uses the existing `catalog_search` tool and existing Catalog Application `SearchProductsQuery`.
- First version searches SKU and Name through existing Catalog search support; description search was not added.
- Public assistant CQRS catalog search is active-only with `IsActive = true` and returns at most 10 products.
- `CatalogAssistantSubAgent` handles broad catalog search and returns existing `catalogProducts` structured data through `AssistantCatalogProductsData` and `AssistantProductCardDto`.
- Text-to-SQL internals, feature flag behavior, planner, validator, executor, and mapper were unchanged.
- `AssistantQueryResponse`, frontend contracts, MCP, database schema, migrations, appsettings secrets, raw SQL exposure, `genericTable` exposure, and admin/write assistant behavior were unchanged.
- Added demo slide source `docs/demo/features/assistant-broad-catalog-search-demo-slides.md`.
- `dotnet restore Ecommerce.sln`: passed with NU1900 warnings for unreachable vulnerability feeds.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\broad-catalog-search-build`: passed with NU1900 warnings.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\broad-catalog-search-test`: passed with NU1900 warnings.
- Catalog unit tests: 83 passed.
- Auth unit tests: 68 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 182 passed.

Latest code execution after Assistant Product Detail By Search:

- Added read-only `CatalogGetProductBySearch` intent for natural product detail and price questions by name, SKU, or search text.
- `CatalogAssistantSubAgent` searches with `SearchProductsQuery(searchText, true, 1, 2)` and never guesses between multiple active matches.
- Zero matches return a supported empty `catalogProducts` response; one active match is resolved through `GetProductByIdQuery` and returns the existing `catalogProduct` response; multiple matches return up to two existing `catalogProducts` choices.
- Unique detail results are rechecked for active status before exposure. Missing or concurrently inactive details return the supported empty choices response.
- Existing `AssistantQueryResponse`, structured response DTOs, frontend contracts, Catalog Application queries, tool names, MCP, Orders assistant behavior, database schema, migrations, and packages were unchanged.
- Text-to-SQL internals, prompts, validator, executor, views, mapper, configuration, and feature flags were unchanged; existing safe fallback can reach the new CQRS path.
- Write/admin requests remain unsupported, and raw SQL and `genericTable` remain unexposed.
- Added demo slide source `docs/demo/features/assistant-product-detail-by-search-demo-slides.md`.
- `dotnet restore Ecommerce.sln`: passed with NU1900 warnings for unreachable vulnerability feeds.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\product-detail-by-search-build`: passed with NU1900 warnings.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\product-detail-by-search-test`: passed with NU1900 warnings.
- Catalog unit tests: 83 passed.
- Auth unit tests: 68 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 209 passed.
- Manual API verification passed for unique, zero, and multiple matches; broad search and price-filter regressions; inactive exclusion; write/admin refusal; and raw SQL/`genericTable` non-exposure.

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
- Auth refresh token, protected Catalog read endpoint, token persistence features, public admin registration endpoint, or admin management UI
- MCP tools beyond the approved initial allowlist
- committed LLM API keys/secrets, provider SDK packages, live provider calls in tests, assistant write actions, assistant raw SQL/database access, assistant admin analytics, or assistant cross-user access
