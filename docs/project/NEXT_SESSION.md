# Next Session Resume Guide

**Last Updated:** 2026-06-24

This file is designed to allow a future AI session to resume project work in less than 5 minutes.

---

## Last Completed Work

- Auth module skeleton creation (Domain, Application, Infrastructure, Contracts projects)
- Auth User aggregate domain model (`User`, `UserId`, `Email`, `PasswordHash`)
- Auth Register User Application layer (`RegisterUserCommand`, handler, validator, result, abstractions)
- Auth Register User persistence and API endpoint (`POST /api/auth/users/register`)
- Auth Login User Application layer (`LoginUserCommand`, handler, validator, result, abstractions)
- Auth Login User persistence and API endpoint (`POST /api/auth/users/login`)
- Auth JWT access token generation for successful login
- Auth JWT bearer validation and protected Current User endpoint (`GET /api/auth/users/me`)
- Swagger JWT authorization and protected Catalog write endpoints
- Catalog Update Product Details (`PUT /api/catalog/products/{productId}`)
- Catalog Product Price Write Support (`price` on create/search/details)
- Catalog Reactivate Product (`POST /api/catalog/products/{productId}/reactivate`)
- Platform Health Checks (`GET /health/live`, `GET /health/ready`)
- Health Checks Documentation Finalization
- Platform Structured Logging (`X-Correlation-ID`, request/error/health/auth logging)
- Orders Initial Vertical Slice (`POST /api/orders`, `GET /api/orders/{orderId}`)
- Orders List For Current User (`GET /api/orders`)
- MCP Server Integration (`POST /mcp`, protected allowlisted MCP tools)
- Ecommerce Assistant Agent Phase 1 (`POST /api/assistant/query`, protected deterministic read-only orchestration)
- Assistant Intent Interpreter Phase 2 (`IAssistantIntentInterpreter`, deterministic default, untrusted plan validation, fake interpreter tests only)
- Assistant LLM Provider Integration Phase 3 (`LlmAssistantIntentInterpreter`, config-gated provider options, HTTP client adapter, fake provider tests only)
- Backend Gemini LLM Provider (`GeminiAssistantLlmClient`, selectable via configuration, POC/testing only)
- Assistant LLM Configuration Diagnostics (temporary safe boolean/presence logs for LLM config, provider failure, fallback, and validation failure)
- Assistant Text-to-SQL read-only database boundary (`assistant` schema/views and manual read-only DB setup docs only)
- Assistant Text-to-SQL SQL validator and read-only executor behind disabled feature flag
- Backend Admin Product Management (`RequireAdmin`, Auth role claims, Admin-only Catalog writes, product price update)
- Swagger Authentication Integration
- Revisit Swagger/API Authentication Integration
- JWT default authentication scheme fix
- AI project memory documentation structure (PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md)
- AGENT.md router implementation with detailed instruction file split
- Architecture rules and testing enforcement
- Instruction file consolidation (V2 rule set)
- Prompt standardization and reusable prompt template setup (`docs/project/PROMPT_TEMPLATE.md`)
- Prompt template compliance contract enhancement for complete short-prompt plans
- Feature demo slide deliverable workflow (`docs/demo/features/{feature-slug}-demo-slides.md`)

---

## Current Repository State

### Solution Structure

All approved projects exist and build successfully:

**BuildingBlocks:**
- `src/BuildingBlocks/Ecommerce.BuildingBlocks.Domain`
- `src/BuildingBlocks/Ecommerce.BuildingBlocks.Application`
- `src/BuildingBlocks/Ecommerce.BuildingBlocks.Infrastructure`

**Modules:**
- `src/Modules/Catalog/*` (Domain, Application, Infrastructure, Contracts)
- `src/Modules/Auth/*` (Domain, Application, Infrastructure, Contracts)
- `src/Modules/Orders/*` (Domain, Application, Infrastructure, Contracts)

**API & Tests:**
- `src/Api/Ecommerce.Api`
- `tests/ArchitectureTests/Ecommerce.ArchitectureTests`
- `tests/UnitTests/Ecommerce.Catalog.UnitTests`
- `tests/UnitTests/Ecommerce.Auth.UnitTests`
- `tests/UnitTests/Ecommerce.Orders.UnitTests`

### Platform Health Checks

- `GET /health/live` reports process liveness only and does not depend on database connectivity.
- `GET /health/ready` reports readiness for Auth, Catalog, and Orders database connectivity.
- Readiness checks use existing EF Core DbContexts with `Database.CanConnectAsync`.
- Health checks do not create databases, apply migrations, or change schema.

### Platform Structured Logging

- Uses built-in ASP.NET Core logging only.
- `X-Correlation-ID` is accepted, preserved when supplied, generated when missing, and returned on every response.
- Request logging records method, path, status code, and elapsed time.
- Exception, health readiness failure, and JWT authentication failure/challenge logging are structured.
- Tokens, authorization headers, passwords, request bodies, and response bodies must not be logged.

### Platform MCP Integration

- Uses `ModelContextProtocol.AspNetCore` in the API project.
- `POST /mcp` is hosted as a protected stateless Streamable HTTP MCP endpoint.
- MCP implementation lives under `src/Api/Ecommerce.Api/Mcp`.
- MCP tools dispatch existing Application/CQRS requests through `ISender`.
- Approved tool allowlist:
  - `catalog_search_products`
  - `catalog_get_product_by_id`
  - `orders_get_order_by_id`
  - `orders_create_order`
- `orders_create_order` requires explicit `confirmedByUser` input.
- Orders MCP tools use the authenticated JWT `sub` claim for owner context.
- MCP does not expose Auth register/login, JWTs, passwords, authorization headers, raw database access, migrations, health readiness details, appsettings, environment variables, SQL, Catalog writes, cross-user orders, or non-existent Orders features.

### Platform Assistant Orchestration

- `POST /api/assistant/query` is protected by bearer authentication.
- The request accepts a natural-language `question` only; it does not accept `userId`, `buyerId`, or caller-selected tools.
- Implementation lives under `src/Api/Ecommerce.Api/Assistant` and `src/Api/Ecommerce.Api/Controllers/Assistant`.
- Assistant intent interpretation goes through `IAssistantIntentInterpreter`; disabled-mode production DI resolves `DeterministicAssistantIntentInterpreter` by default.
- `LlmAssistantIntentInterpreter` is available behind `Assistant:Llm:Enabled` and uses `IAssistantLlmClient`, `HttpClientFactory`, and `System.Text.Json`.
- Provider selection is backend configuration only. `Assistant:Llm:Provider` defaults to `OpenAI`; `ECOMMERCE_ASSISTANT_LLM_PROVIDER=Gemini` selects `GeminiAssistantLlmClient`.
- Gemini is a POC/testing provider. Configure it with `ECOMMERCE_ASSISTANT_GEMINI_API_KEY`, optional `ECOMMERCE_ASSISTANT_GEMINI_MODEL` (default `gemini-2.5-flash`), and optional `ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT` (default `https://generativelanguage.googleapis.com/v1beta`).
- Gemini free-tier and rate-limit behavior varies by Google account/project. A ChatGPT/OpenAI subscription is unrelated to Gemini Developer API access.
- `Assistant:Llm` settings are committed only with non-secret values. API keys must come from the configured environment variable or user secrets/non-committed configuration providers.
- Interpreter output is an untrusted `AssistantIntentPlan` that must pass `AssistantIntentPlanValidator` before any execution.
- The validator checks intent kind, exact read-only tool plan, allowed arguments, unsafe request terms, and model-provided scope arguments.
- Temporary LLM diagnostics log only safe booleans/presence flags for config, provider call failure, deterministic fallback usage, and model output validation failure.
- Do not log prompts, raw provider responses, API keys, tokens, auth headers, full Gemini request URIs, or sensitive payloads.
- Fake/test interpreters and fake provider clients are used in tests only; no provider SDK package, committed API key, committed secret, live test call, runtime database access, or MCP dependency has been added for LLM provider execution.
- Approved capability names are `catalog_search`, `catalog_get_product`, `orders_search`, `orders_get_order`, and `orders_analyze`.
- Assistant code dispatches existing Catalog and Orders read-side CQRS queries through `ISender`.
- Assistant code must not call EF Core DbContexts, repositories, Domain objects, module internals, write commands, or MCP protocol packages directly.
- Orders assistant analysis is owner-scoped to the authenticated JWT `sub` claim.
- Unsafe, mutating, admin, SQL, token, database, internal, unclear, and cross-user requests return safe unsupported responses.
- Task 2 added a dormant Text-to-SQL SQL validator and read-only executor behind `Assistant:TextToSql:Enabled`, which defaults to `false`.
- The LLM SQL planner and assistant orchestration wiring are not implemented yet, so existing assistant endpoint behavior is unchanged.
- Future Text-to-SQL execution must use `ConnectionStrings:AssistantCatalogReadOnly` for Catalog views and `ConnectionStrings:AssistantOrdersReadOnly` for Orders views. Each read-only principal should have `SELECT` only on the `assistant` schema/views in its own database and no direct base-table permissions.
- Do not use normal application DB connection strings for Text-to-SQL execution.
- ADR-004 documents the assistant orchestration boundary and safety model.
- ADR-005 documents untrusted assistant intent interpretation and plan validation.
- ADR-006 documents config-gated LLM provider integration.

### Database Status

- **Persistence:** SQL Server LocalDB `(localdb)\mssqllocaldb`
- **Connection Strings:** `ConnectionStrings:Catalog`, `ConnectionStrings:Auth`, and `ConnectionStrings:Orders` in `appsettings.Development.json`
- **Catalog Migrations:** `20260608111338_InitialCatalogSchema.cs`, `20260618090000_AddProductPrice.cs`
- **Auth Migrations:** `20260609092505_InitialAuthSchema.cs`, `20260623090000_AddUserRole.cs`
- **Orders Migration:** `20260612090403_InitialOrdersSchema.cs`
- **Assistant Catalog View Migration:** `20260624090000_AddAssistantCatalogReadOnlyViews.cs` in the Catalog migration path
- **Assistant Orders View Migration:** `20260624090000_AddAssistantReadOnlyViews.cs` in the Orders migration path
- **Auth Persistence:** `EcommerceAuth` LocalDB database updated through `InitialAuthSchema`
- **Orders Persistence:** `EcommerceOrders` LocalDB database created and updated through `InitialOrdersSchema`
- **Assistant View Boundary:** Catalog and Orders views are created separately in their owning databases. Do not use cross-database views, linked servers, synonyms, or a combined Catalog/Orders database assumption for Task 1.

### Build & Test Status

Last verified pass (2026-06-19):
```
dotnet restore Ecommerce.sln               PASSED
dotnet build Ecommerce.sln                 PASSED
dotnet test Ecommerce.sln                  PASSED
  - Catalog Unit Tests: 75 passed
  - Auth Unit Tests: 65 passed
  - Orders Unit Tests: 23 passed
  - Architecture Tests: 88 passed
```

---

## Current Architecture

### High Level

- **Pattern:** Clean Architecture Modular Monolith
- **API Layer:** ASP.NET Core Web API with thin controllers
- **Application Layer:** CQRS with MediatR + FluentValidation
- **Domain Layer:** DDD with value objects (Sku, ProductName, ProductId)
- **Infrastructure:** EF Core with SQL Server backend
- **Testing:** xUnit for unit tests, ArchitectureTests for dependency enforcement

### Dependency Flow (Enforced)

```
API
  ↓
Application
  ↓
Domain

Infrastructure → Application → Domain

BuildingBlocks → (must not reference modules)
Catalog/Auth/Orders (no cross-module references)
```

### Module Isolation

- Each module owns: Domain, Application, Infrastructure, Contracts
- Cross-module communication through Contracts only
- No internal project references between modules
- Catalog, Auth, and Orders must not reference each other

### Catalog Module (Complete)

**Implemented Features:**
- Create Product (POST /api/catalog/products, Admin-only)
- Get Product By Id (GET /api/catalog/products/{productId})
- Search/List Products with pagination (GET /api/catalog/products)
- Update Product Details (PUT /api/catalog/products/{productId}, Admin-only)
- Update Product Price (PUT /api/catalog/products/{productId}/price, Admin-only)
- Deactivate Product (DELETE /api/catalog/products/{productId}, Admin-only)
- Reactivate Product (POST /api/catalog/products/{productId}/reactivate, Admin-only)

**Key Decisions:**
- Product search uses infrastructure read model (`ProductSearchReadModel`, `CatalogReadDbContext`)
- See: `docs/decisions/ADR-001-product-search-read-model.md`
- Do NOT revert to aggregate value object access inside EF queries
- Product price is Catalog-owned aggregate state. Create Product accepts non-negative `price`, persists it as `decimal(18,2)`, and search/details responses return it. Update Product Details does not change price.
- Catalog write endpoints require a valid bearer token with role `Admin`.
- Catalog read endpoints remain public.
- Product reactivation is idempotent and does not update `UpdatedAt` when the product is already active.

**Entities:**
- `Product` (aggregate root)
- Value objects: `ProductId`, `Sku`, `ProductName`
- DTOs: `ProductDto`, `PaginatedProductsDto`

### Auth Module (Register User + Login User API, Persistence, JWT Access Token, And Current User Endpoint)

**Current State:**
- Projects exist: Domain, Application, Infrastructure, Contracts, UnitTests
- Domain model exists: `User`, `UserId`, `Email`, `PasswordHash`, `UserRole`
- Domain behaviors exist: Register, VerifyEmail, ChangePassword, Deactivate, Reactivate
- Application registration use case exists: `RegisterUserCommand`, handler, validator, result, duplicate email exception, and abstractions
- Application login use case exists: `LoginUserCommand`, handler, validator, result, invalid credentials exception, inactive user exception, and abstractions
- Infrastructure support exists: `AuthDbContext`, `UserConfiguration`, `UserRepository`, `PasswordHasher`, and DI registration
- JWT access token support exists: `IAccessTokenGenerator`, `AccessTokenResult`, `JwtOptions`, `JwtAccessTokenGenerator`
- JWT tokens include a `role` claim and registered users default to role `Customer`.
- Local Admin users are promoted through documented development database update only; no public admin registration endpoint exists.
- JWT bearer validation is configured in the API using existing `Auth:Jwt` settings, with explicit default authenticate and challenge schemes
- Authentication and authorization middleware are registered
- Swagger/OpenAPI uses the standard HTTP bearer scheme and per-operation security metadata for `[Authorize]` actions. Enter the raw JWT access token in Swagger Authorize; Swagger UI sends `Authorization: Bearer {token}`. Swagger UI authorization persistence is enabled.
- Contracts exist: `RegisterUserRequest`, `RegisterUserResponse`, `LoginUserRequest`, `LoginUserResponse`, `GetCurrentUserResponse`
- `GetCurrentUserResponse` returns `userId`, `email`, and `role`.
- API endpoints exist: `POST /api/auth/users/register`, `POST /api/auth/users/login`, `GET /api/auth/users/me`
- Manual Auth migration exists: `20260609092505_InitialAuthSchema.cs`

**Intentionally Absent:**
- Refresh token strategy
- Token persistence
- Public admin registration endpoint
- Broader permissions beyond Customer/Admin

Do NOT add these until explicitly approved with APPROVED: EXECUTE.

### Orders Module

**Implemented Features:**
- Create Order (POST /api/orders, protected)
- List Orders For Current User (GET /api/orders, protected, owner-scoped summaries only)
- Get Order By Id (GET /api/orders/{orderId}, protected and owner-scoped)

**Key Decisions:**
- Orders uses product snapshot data supplied in the create request for this slice.
- See: `docs/decisions/ADR-002-orders-product-snapshot-strategy.md`
- Orders does not reference Catalog or Auth internals.
- All Orders endpoints require a valid bearer token.
- Users may only list or retrieve their own orders.
- `GET /api/orders` uses `pageNumber` and `pageSize`, defaults to `1` and `20`, limits `pageSize` to `100`, sorts newest first by `CreatedAt`, and returns only `orderId`, `status`, `totalAmount`, `createdAt`, and `lineCount`.
- Frontend contract notes live in `docs/project/FRONTEND_CONTRACT.md`.
- Initial order lifecycle is intentionally limited to `Created`.

**Entities:**
- `Order` (aggregate root)
- `OrderLine` (child entity with product snapshot data)
- Value objects: `OrderId`, `OrderLineId`, `BuyerId`
- Enum: `OrderStatus`

**Intentionally Absent:**
- Payments
- Inventory reservation
- Shipping
- Discounts/coupons
- Cancellation/refunds
- Advanced order status workflows
- Customer profile integration
- Additional Orders MCP tools beyond the approved allowlist

### BuildingBlocks

**Purpose:** Shared abstractions that do not reference any module

**Current Contents:**
- Domain: Generic interfaces and base classes
- Application: Generic command/query base types
- Infrastructure: Generic persistence abstractions

---

## Current Phase

**Phase:** Awaiting Next Approved Task

**Completed Phases:**
1. Solution skeleton
2. Architecture tests foundation
3. Clean Architecture enforcement
4. Catalog business features (CRUD operations)
5. Database persistence
6. Global error handling
7. Auth module skeleton
8. Auth User aggregate domain model
9. Auth Register User Application layer
10. Auth Register User persistence and API endpoint
11. Auth Login User Application layer
12. Auth Login User persistence and API endpoint
13. Auth JWT access token generation
14. Auth JWT bearer validation and protected Current User endpoint
15. Swagger JWT authorization and protected Catalog write endpoints
16. JWT default authentication scheme fix
17. Project memory documentation
18. AGENT.md router and instruction file split
19. Prompt standardization and reusable prompt template setup
20. Prompt template compliance contract enhancement
21. Catalog Update Product Details
22. Swagger Authentication Integration
23. Revisit Swagger/API Authentication Integration
24. Catalog Reactivate Product
25. Platform Health Checks
26. Health Checks Documentation Finalization
27. Platform Structured Logging
28. Orders Initial Vertical Slice
29. Orders List For Current User
30. Catalog Product Price Write Support
31. MCP Server Integration
32. Product Knowledge Documentation Pack
33. Ecommerce Assistant Agent Phase 1
34. Assistant Intent Interpreter Phase 2
35. Assistant LLM Provider Integration Phase 3
36. Assistant LLM Configuration Diagnostics
37. Backend Admin Product Management
38. Backend Gemini LLM Provider

**In Progress:**
- Maintaining NEXT_SESSION.md after every execution task

**Next Phases (When Explicitly Approved):**
- Additional Catalog features (product uniqueness hardening, etc.)
- Auth refresh token planning
- Auth broader protected endpoint authorization planning
- Orders Catalog validation/integration planning
- MCP frontend integration planning
- MCP authorization policy/rate limiting planning
- Assistant frontend integration planning
- Assistant provider-specific production configuration or operational smoke testing
- Gemini POC demo validation with real account/project rate-limit expectations
- Catalog write authorization policy refinement
- Integration testing setup
- Platform features (API versioning, configuration validation, etc.)

---

## Next Approved Task

**There is no currently approved task.**

The last completed work was Backend Admin Product Management. Wait for explicit user direction with APPROVED: EXECUTE before beginning any new execution work.

**How to Proceed:**

1. Read the "Required Reading Order" section below
2. Ask the user: "What should we work on next?"
3. Wait for explicit approval: `APPROVED: EXECUTE <task description>`
4. Follow the planning and execution workflow from `instructions/01-execution-and-planning.md`

---

## Required Reading Order

When resuming in a new session, read these files in this exact order:

1. `AGENT.md` (router and entry point)
2. `instructions/00-role-and-stack.md` (role, stack, architecture strategy)
3. `instructions/01-execution-and-planning.md` (state machine, execution lock, planning rules)
4. `instructions/02-architecture-and-modules.md` (Clean Architecture, module rules)
5. `instructions/03-cqrs-database-testing-security.md` (CQRS, database, testing, security rules)
6. `instructions/04-documentation-and-memory.md` (prompt logging, project memory rules)
7. `instructions/05-completion.md` (self-review and task completion rules)
8. `docs/project/PROMPT_TEMPLATE.md` (reusable short-prompt workflow)
9. `docs/project/PROJECT_STATUS.md` (current implementation snapshot)
10. `docs/project/ROADMAP.md` (completed and candidate work)
11. Latest files in `docs/prompts/` (recent work context)
12. Relevant ADRs in `docs/decisions/` (architectural decisions)

**Expected Time:** ~5 minutes to read all

---

## Resume Prompt

Use this prompt to orient yourself when starting a new session:

> I need to resume the e-commerce backend project. What is the current state and what should I do next?
>
> I've already read:
> - docs/project/NEXT_SESSION.md
> - docs/project/PROJECT_STATUS.md
> - docs/project/AI_HANDOFF.md
>
> Verify the current state and suggest what work to prioritize next.

---

## Warnings

### Critical: Execution Lock

**Do not write code, create files, or run commands without explicit approval.**

Approval phrase: `APPROVED: EXECUTE <task description>`

Without this phrase, all execution is forbidden. This gate prevents accidental code generation.

### Critical: Prompt Logging

All architecture, planning, execution, testing, and documentation work must be logged.

**Location:** `docs/prompts/`

**Format:** `NNN-title.md` (001, 002, 003, ...)

**Contents:**
- Prompt number and date
- Purpose
- Full prompt text
- Approval status (PLANNED, APPROVED, EXECUTED, FAILED)
- Result summary

**Exception:** User may write `SKIP PROMPT LOG` to override this requirement for documentation-only work.

### Important: Auth Has Access Tokens And /me Only

The Auth module can register users through `POST /api/auth/users/register`, verify credentials through `POST /api/auth/users/login`, and return the current authenticated user through `GET /api/auth/users/me`. Login returns a short-lived JWT access token with a `role` claim. The `/me` endpoint requires a valid bearer token and returns `userId`, `email`, and `role`. Catalog product write endpoints require a valid Admin bearer token through the `RequireAdmin` policy; Catalog product read endpoints remain public. Swagger has an Authorize button for JWT access tokens; enter the raw JWT access token and Swagger UI sends `Authorization: Bearer {token}`. If generated Swagger curl lacks the `Authorization` header, restart the API process and refresh Swagger because the running process may be serving stale OpenAPI JSON.

Do NOT add:
- Refresh tokens
- Protected Catalog read endpoints
- Token persistence
- Public admin registration endpoint

Wait for explicit architectural approval before auth business features.

### Important: Catalog Search Read Model

Do NOT revert Catalog product search back to querying aggregate properties inside EF expressions.

Current implementation uses `ProductSearchReadModel` and `CatalogReadDbContext`.

**See:** `docs/decisions/ADR-001-product-search-read-model.md`

This decision is documented and locked in. Reverting would require an ADR update and explicit approval.

### Important: No Migrations Without Approval

SQL Server LocalDB uses:
- Catalog connection string: `ConnectionStrings:Catalog`
- Auth connection string: `ConnectionStrings:Auth`
- Orders connection string: `ConnectionStrings:Orders`
- Catalog migrations: `20260608111338_InitialCatalogSchema.cs`, `20260618090000_AddProductPrice.cs`
- Auth migration: `20260609092505_InitialAuthSchema.cs`
- Orders migration: `20260612090403_InitialOrdersSchema.cs`

Do NOT create migrations or schema changes without explicit approval.

### Important: Health Checks Are Platform-Only

Current health endpoints are:
- `GET /health/live` for process liveness only.
- `GET /health/ready` for Auth, Catalog, and Orders database readiness.

The live endpoint must not depend on database connectivity. The ready endpoint must not create databases, apply migrations, or change schema.

### Important: Orders Product Snapshot Strategy

Orders stores product snapshot data from the create request for the initial vertical slice.

**See:** `docs/decisions/ADR-002-orders-product-snapshot-strategy.md`

Do NOT add direct Orders references to Catalog or Auth internals. Do NOT add payments, inventory reservation, shipping, discounts, coupons, cancellation, refunds, advanced order status workflows, or Customer profile integration without explicit approval.

`GET /api/orders` must remain owner-scoped to the JWT `sub` claim and must return summaries only. Do not add full order lines to the list response; use `GET /api/orders/{orderId}` for line details.

### Important: MCP Is API-Layer Only

MCP is implemented under `src/Api/Ecommerce.Api/Mcp` and must remain an API/platform adapter.

Current MCP endpoint:
- `POST /mcp` protected by bearer authentication.

Current tool allowlist:
- `catalog_search_products`
- `catalog_get_product_by_id`
- `orders_get_order_by_id`
- `orders_create_order`

`orders_create_order` must require explicit `confirmedByUser` input. MCP handlers must call existing Application/CQRS requests through `ISender`. Do NOT call EF Core DbContexts, repositories, Domain objects, or module internals directly from MCP code.

Do NOT expose:
- Auth register/login
- JWTs
- passwords
- authorization headers
- raw database access
- migrations
- health readiness details
- appsettings
- environment variables
- SQL
- Catalog writes
- cross-user orders
- non-existent Orders features
- tools beyond the approved allowlist

### Important: Assistant Is Read-Only API Orchestration

Current assistant endpoint:
- `POST /api/assistant/query` protected by bearer authentication.

Current capability allowlist:
- `catalog_search`
- `catalog_get_product`
- `orders_search`
- `orders_get_order`
- `orders_analyze`

Assistant handlers must call existing read-side Application/CQRS requests through `ISender`. Do NOT call EF Core DbContexts, repositories, Domain objects, module internals, write commands, or MCP protocol packages from assistant code. Do NOT accept `userId` or `buyerId` from the request body. Orders analysis must use the JWT `sub` claim.

Interpreter output is untrusted. The LLM provider must return only structured intent/tool plans, and those plans must pass deterministic backend validation before execution. Reject unknown tools, unsafe requests, mutating/admin/SQL/cross-user plans, and any model-provided user or buyer scope.

LLM provider secrets must come only from environment variables or user secrets/non-committed configuration providers. Do not commit API keys. Do not log prompts, raw provider responses, tokens, API keys, auth headers, or sensitive payloads. Automated tests must keep using fake provider clients, not live network calls.

Temporary LLM diagnostics are allowed to log only booleans/presence flags and fallback/failure status. Do not expand them to include prompt text, raw provider responses, API key values, auth headers, tokens, or sensitive payloads.

Do NOT expose:
- raw SQL
- database internals
- tokens or authorization headers
- internal prompts/routing details
- mutating actions
- Catalog writes
- Orders writes
- admin analytics
- cross-user data
- provider SDK packages, committed LLM secrets, or live provider calls in automated tests without explicit approval

### Important: Structured Logging Is API/Platform-Only

Current structured logging uses built-in ASP.NET Core logging only. `X-Correlation-ID` must be preserved when supplied and returned on every response.

Do NOT log:
- tokens
- authorization headers
- passwords
- request bodies
- response bodies

Do NOT add logging packages, sinks, schema changes, Domain changes, Application changes, CQRS changes, modules, or business behavior changes without explicit approval.

### Important: AGENT.md is a Router

AGENT.md is no longer the full rule set. It is now a router that points to detailed files under `instructions/`.

When AGENT.md changes, always replace the **entire** file. Do not provide or apply partial AGENT.md edits.

When instruction files change, preserve all still-valid rules. If project operating rules change, update project memory.

### Important: Reusable Prompt Template

Short prompts are supported through `docs/project/PROMPT_TEMPLATE.md`.

Examples:
- `Plan next Catalog feature: Update Product Details`
- `APPROVED: EXECUTE Execute approved feature: Update Product Details`

Short planning prompts must return every section from the template's Plan Output Contract using exact section names, then internally run the Plan Self-Validation Rule before responding.

Short prompts do not override the execution lock, prompt logging, Clean Architecture, DDD, CQRS, module isolation, testing, security, or completion rules.

For main features, major platform capabilities, API modules, integrations, or demo-worthy backend behavior, future approved executions must create or update `docs/demo/features/{feature-slug}-demo-slides.md`. The slide source must be presentation-ready Markdown with required sections, Mermaid diagrams where useful, and `Speaker cue:` lines. Tiny fixes, typo fixes, internal refactors with no demo value, prompt-template-only cleanup, test-only cleanup, and documentation-only maintenance do not require slide files unless explicitly requested.

Historical prompt logs under `docs/prompts/` must not be rewritten for style cleanup.

### Important: No Bootstrapper or Shared Projects

Do NOT create:
- Bootstrapper project (no auto-migrations or startup logic)
- Shared project (encourages bad dependencies)
- Microservices or event bus
- Distributed transactions

These require explicit architectural approval and an ADR.

### Important: No Cross-Module References

Catalog and Auth must not reference each other.

Cross-module communication must happen through:
- Contracts layer only
- Public abstractions only
- Explicit integration mechanisms

Never through internal project references.

---

## Quick Verification

When resuming in a new session, verify the current state with:

```powershell
# Restore and build
dotnet restore Ecommerce.sln
dotnet build Ecommerce.sln

# Run all tests
dotnet test Ecommerce.sln

# Expected output:
# - Build: success
# - Catalog Unit Tests: 83 passed
# - Auth Unit Tests: 68 passed
# - Orders Unit Tests: 23 passed
# - Architecture Tests: 101 passed
```

If a local API process is running and locks `src/Api/Ecommerce.Api/bin/Debug/net9.0/Ecommerce.Api.dll`, either stop that process intentionally or verify with isolated artifacts:

```powershell
dotnet build Ecommerce.sln --artifacts-path artifacts\assistant-verify
dotnet test Ecommerce.sln --artifacts-path artifacts\assistant-test
```

If build or tests fail, check:
1. SQL Server LocalDB is running: `sqllocaldb start mssqllocaldb`
2. Connection string in `src/Api/Ecommerce.Api/appsettings.json` points to `(localdb)\mssqllocaldb`
3. Latest Catalog migration applied: `dotnet ef database update --project src/Modules/Catalog/Ecommerce.Catalog.Infrastructure`
4. Latest Auth migration applied: `dotnet ef database update --project src/Modules/Auth/Ecommerce.Auth.Infrastructure --startup-project src/Api/Ecommerce.Api --context AuthDbContext`
5. Latest Orders migration applied: `dotnet ef database update --project src/Modules/Orders/Ecommerce.Orders.Infrastructure --startup-project src/Api/Ecommerce.Api --context OrdersDbContext`

---

## Key Documentation References

| Topic | Location |
|-------|----------|
| Architecture Strategy | `instructions/00-role-and-stack.md#current-architecture-strategy` |
| Execution Lock | `instructions/01-execution-and-planning.md#execution-lock` |
| Reusable Prompt Template | `docs/project/PROMPT_TEMPLATE.md` |
| Feature Demo Slides | `docs/demo/features/` |
| CQRS Rules | `instructions/03-cqrs-database-testing-security.md#cqrs-rules` |
| DDD Ownership Rules | `instructions/03-cqrs-database-testing-security.md#ddd-ownership-rules` |
| Module Isolation | `instructions/02-architecture-and-modules.md#module-rules` |
| Project Memory | `instructions/04-documentation-and-memory.md#ai-project-memory-rule` |
| Product Search Decision | `docs/decisions/ADR-001-product-search-read-model.md` |
| Orders Product Snapshot Decision | `docs/decisions/ADR-002-orders-product-snapshot-strategy.md` |
| MCP Boundary And Security Decision | `docs/decisions/ADR-003-mcp-server-boundary-and-security.md` |
| Assistant Boundary And Safety Decision | `docs/decisions/ADR-004-assistant-orchestration-boundary-and-safety.md` |
| Assistant Untrusted Intent Interpretation Decision | `docs/decisions/ADR-005-assistant-untrusted-intent-interpretation.md` |
| Assistant LLM Provider Integration Decision | `docs/decisions/ADR-006-assistant-llm-provider-integration.md` |
| Frontend Contract | `docs/project/FRONTEND_CONTRACT.md` |
| Recent Prompts | `docs/prompts/` (start with highest numbers) |

---

## Maintenance

**NEXT_SESSION.md must be updated after every completed execution task.**

Update these sections when:
- **Last Completed Work:** After any execution task changes state
- **Current Repository State:** After code or schema changes
- **Current Architecture:** After architectural decisions
- **Current Phase:** After phase transitions
- **Next Approved Task:** After user provides new approval
- **Warnings:** When new constraints are discovered
- **Build & Test Status:** After execution that changes build/test outcomes

See `instructions/04-documentation-and-memory.md#ai-project-memory-rule` for the full rule.
