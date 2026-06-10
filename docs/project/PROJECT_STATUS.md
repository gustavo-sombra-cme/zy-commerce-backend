# Project Status

## Snapshot

Date: 2026-06-09

The repository is a .NET 9 ASP.NET Core backend for an enterprise e-commerce system. It follows Clean Architecture First, Modular Monolith, Module Isolation, CQRS, and thin controller rules.

Agent operating rules are split between a short `AGENT.md` router and detailed files under `instructions/`.

**Project Memory Files:**

- `docs/project/PROJECT_STATUS.md` - Current implementation snapshot
- `docs/project/AI_HANDOFF.md` - Constraints and architecture for new sessions
- `docs/project/ROADMAP.md` - Completed and candidate work
- `docs/project/NEXT_SESSION.md` - Fast resume guide (< 5 minutes to read)
- `docs/project/PROMPT_TEMPLATE.md` - Reusable planning and execution prompt defaults for shorter future prompts

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
- Update Product Details
- Deactivate Product

Catalog uses:

- DDD aggregate behavior for `Product`
- CQRS with MediatR
- FluentValidation
- API authorization on write endpoints
- EF Core persistence
- SQL Server LocalDB
- Manual EF Core migration: `InitialCatalogSchema`
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

## API Status

Catalog endpoints are implemented through controllers under:

- `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`

Current product routes:

- `POST /api/catalog/products` - protected, requires valid bearer token
- `GET /api/catalog/products/{productId}`
- `GET /api/catalog/products`
- `PUT /api/catalog/products/{productId}` - protected, requires valid bearer token
- `DELETE /api/catalog/products/{productId}` - protected, requires valid bearer token

Current Auth routes:

- `POST /api/auth/users/register`
- `POST /api/auth/users/login`
- `GET /api/auth/users/me`

Swagger/OpenAPI is enabled for local Development only. Swagger uses the standard HTTP bearer security scheme and includes an Authorize button. Protected operations are marked with per-operation security metadata from `[Authorize]`, while public endpoints remain public. In Swagger Authorize, enter the raw JWT access token only; Swagger UI sends `Authorization: Bearer {token}`. Swagger UI persists authorization across browser refreshes.

## Database Status

Catalog has EF Core persistence and one approved migration:

- `src/Modules/Catalog/Ecommerce.Catalog.Infrastructure/Persistence/Migrations/20260608111338_InitialCatalogSchema.cs`

Auth has EF Core persistence and one approved migration:

- `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Persistence/Migrations/20260609092505_InitialAuthSchema.cs`

Local development uses:

- `ConnectionStrings:Catalog`
- `ConnectionStrings:Auth`
- SQL Server LocalDB
- `(localdb)\mssqllocaldb`

The Auth LocalDB database `EcommerceAuth` has been updated through `InitialAuthSchema`.

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
- BuildingBlocks projects do not reference Catalog or Auth.
- Catalog and Auth do not reference each other.
- Catalog write endpoints require authorization.
- Catalog read endpoints remain public.
- Auth register and login remain public.
- Auth current-user endpoint remains protected.
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
- Auth refresh token, roles, permissions, protected Catalog read endpoint, or token persistence features
