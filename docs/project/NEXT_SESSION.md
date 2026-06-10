# Next Session Resume Guide

**Last Updated:** 2026-06-09

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
- Swagger Authentication Integration
- Revisit Swagger/API Authentication Integration
- JWT default authentication scheme fix
- AI project memory documentation structure (PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md)
- AGENT.md router implementation with detailed instruction file split
- Architecture rules and testing enforcement
- Instruction file consolidation (V2 rule set)
- Prompt standardization and reusable prompt template setup (`docs/project/PROMPT_TEMPLATE.md`)
- Prompt template compliance contract enhancement for complete short-prompt plans

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

**API & Tests:**
- `src/Api/Ecommerce.Api`
- `tests/ArchitectureTests/Ecommerce.ArchitectureTests`
- `tests/UnitTests/Ecommerce.Catalog.UnitTests`
- `tests/UnitTests/Ecommerce.Auth.UnitTests`

### Database Status

- **Persistence:** SQL Server LocalDB `(localdb)\mssqllocaldb`
- **Connection Strings:** `ConnectionStrings:Catalog` and `ConnectionStrings:Auth` in `appsettings.Development.json`
- **Catalog Migration:** `20260608111338_InitialCatalogSchema.cs`
- **Auth Migration:** `20260609092505_InitialAuthSchema.cs`
- **Auth Persistence:** `EcommerceAuth` LocalDB database updated through `InitialAuthSchema`

### Build & Test Status

Last verified pass (2026-06-09):
```
dotnet restore Ecommerce.sln    ✓ PASSED
dotnet build Ecommerce.sln       ✓ PASSED
dotnet test Ecommerce.sln        ✓ PASSED
  - Catalog Unit Tests: 61 passed
  - Architecture Tests: 24 passed
  - Auth Unit Tests: 65 passed
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
Catalog ← ↛ Auth (no cross-module references)
```

### Module Isolation

- Each module owns: Domain, Application, Infrastructure, Contracts
- Cross-module communication through Contracts only
- No internal project references between modules
- Catalog and Auth must not reference each other

### Catalog Module (Complete)

**Implemented Features:**
- Create Product (POST /api/catalog/products, protected)
- Get Product By Id (GET /api/catalog/products/{productId})
- Search/List Products with pagination (GET /api/catalog/products)
- Update Product Details (PUT /api/catalog/products/{productId}, protected)
- Deactivate Product (DELETE /api/catalog/products/{productId}, protected)

**Key Decisions:**
- Product search uses infrastructure read model (`ProductSearchReadModel`, `CatalogReadDbContext`)
- See: `docs/decisions/ADR-001-product-search-read-model.md`
- Do NOT revert to aggregate value object access inside EF queries
- Catalog write endpoints require a valid bearer token.
- Catalog read endpoints remain public.

**Entities:**
- `Product` (aggregate root)
- Value objects: `ProductId`, `Sku`, `ProductName`
- DTOs: `ProductDto`, `PaginatedProductsDto`

### Auth Module (Register User + Login User API, Persistence, JWT Access Token, And Current User Endpoint)

**Current State:**
- Projects exist: Domain, Application, Infrastructure, Contracts, UnitTests
- Domain model exists: `User`, `UserId`, `Email`, `PasswordHash`
- Domain behaviors exist: Register, VerifyEmail, ChangePassword, Deactivate, Reactivate
- Application registration use case exists: `RegisterUserCommand`, handler, validator, result, duplicate email exception, and abstractions
- Application login use case exists: `LoginUserCommand`, handler, validator, result, invalid credentials exception, inactive user exception, and abstractions
- Infrastructure support exists: `AuthDbContext`, `UserConfiguration`, `UserRepository`, `PasswordHasher`, and DI registration
- JWT access token support exists: `IAccessTokenGenerator`, `AccessTokenResult`, `JwtOptions`, `JwtAccessTokenGenerator`
- JWT bearer validation is configured in the API using existing `Auth:Jwt` settings, with explicit default authenticate and challenge schemes
- Authentication and authorization middleware are registered
- Swagger/OpenAPI uses the standard HTTP bearer scheme and per-operation security metadata for `[Authorize]` actions. Enter the raw JWT access token in Swagger Authorize; Swagger UI sends `Authorization: Bearer {token}`. Swagger UI authorization persistence is enabled.
- Contracts exist: `RegisterUserRequest`, `RegisterUserResponse`, `LoginUserRequest`, `LoginUserResponse`, `GetCurrentUserResponse`
- API endpoints exist: `POST /api/auth/users/register`, `POST /api/auth/users/login`, `GET /api/auth/users/me`
- Manual Auth migration exists: `20260609092505_InitialAuthSchema.cs`

**Intentionally Absent:**
- Refresh token strategy
- Roles/permissions
- Token persistence

Do NOT add these until explicitly approved with APPROVED: EXECUTE.

### BuildingBlocks

**Purpose:** Shared abstractions that do not reference any module

**Current Contents:**
- Domain: Generic interfaces and base classes
- Application: Generic command/query base types
- Infrastructure: Generic persistence abstractions

---

## Current Phase

**Phase:** Revisit Swagger/API Authentication Integration

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

**In Progress:**
- Maintaining NEXT_SESSION.md after every execution task

**Next Phases (When Explicitly Approved):**
- Additional Catalog features (Reactivate Product, product uniqueness hardening, etc.)
- Auth refresh token planning
- Auth broader protected endpoint authorization planning
- Catalog write authorization policy refinement
- Integration testing setup
- Platform features (health checks, logging, API versioning, etc.)

---

## Next Approved Task

**There is no currently approved task.**

The last completed work was Revisit Swagger/API Authentication Integration. Wait for explicit user direction with APPROVED: EXECUTE before beginning any new execution work.

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

The Auth module can register users through `POST /api/auth/users/register`, verify credentials through `POST /api/auth/users/login`, and return the current authenticated user through `GET /api/auth/users/me`. Login returns a short-lived JWT access token. The `/me` endpoint requires a valid bearer token and returns only `userId` and `email`. Catalog product write endpoints (`POST`, `PUT`, and `DELETE`) require a valid bearer token; Catalog product read endpoints remain public. Swagger has an Authorize button for JWT access tokens; enter the raw JWT access token and Swagger UI sends `Authorization: Bearer {token}`. If Swagger-generated curl lacks the `Authorization` header, restart the API process and refresh Swagger because the running process may be serving stale OpenAPI JSON.

Do NOT add:
- Refresh tokens
- Roles or permissions
- Protected Catalog read endpoints
- Token persistence

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
- Catalog migration: `20260608111338_InitialCatalogSchema.cs`
- Auth migration: `20260609092505_InitialAuthSchema.cs`

Do NOT create migrations or schema changes without explicit approval.

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
# - Catalog Unit Tests: 61 passed
# - Architecture Tests: 24 passed
# - Auth Unit Tests: 65 passed
```

If build or tests fail, check:
1. SQL Server LocalDB is running: `sqllocaldb start mssqllocaldb`
2. Connection string in `src/Api/Ecommerce.Api/appsettings.json` points to `(localdb)\mssqllocaldb`
3. Latest Catalog migration applied: `dotnet ef database update --project src/Modules/Catalog/Ecommerce.Catalog.Infrastructure`
4. Latest Auth migration applied: `dotnet ef database update --project src/Modules/Auth/Ecommerce.Auth.Infrastructure --startup-project src/Api/Ecommerce.Api --context AuthDbContext`

---

## Key Documentation References

| Topic | Location |
|-------|----------|
| Architecture Strategy | `instructions/00-role-and-stack.md#current-architecture-strategy` |
| Execution Lock | `instructions/01-execution-and-planning.md#execution-lock` |
| Reusable Prompt Template | `docs/project/PROMPT_TEMPLATE.md` |
| CQRS Rules | `instructions/03-cqrs-database-testing-security.md#cqrs-rules` |
| DDD Ownership Rules | `instructions/03-cqrs-database-testing-security.md#ddd-ownership-rules` |
| Module Isolation | `instructions/02-architecture-and-modules.md#module-rules` |
| Project Memory | `instructions/04-documentation-and-memory.md#ai-project-memory-rule` |
| Product Search Decision | `docs/decisions/ADR-001-product-search-read-model.md` |
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
