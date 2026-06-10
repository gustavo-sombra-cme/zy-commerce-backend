# AI Handoff

## Start Here

Use this file when starting a new AI session with limited conversation history. Treat repository files as the source of truth.

Read in this order:

1. `docs/project/NEXT_SESSION.md` (fast resume guide)
2. `AGENT.md`
3. `instructions/00-role-and-stack.md`
4. `instructions/01-execution-and-planning.md`
5. `instructions/02-architecture-and-modules.md`
6. `instructions/03-cqrs-database-testing-security.md`
7. `instructions/04-documentation-and-memory.md`
8. `instructions/05-completion.md`
9. `docs/project/PROMPT_TEMPLATE.md`
10. `docs/project/PROJECT_STATUS.md`
11. `docs/project/ROADMAP.md`
12. Latest files in `docs/prompts/`
13. Relevant ADRs in `docs/decisions/`

## Current Shape

The backend is a Clean Architecture modular monolith on .NET 9.

Current modules:

- Catalog: implemented product features; write endpoints require bearer authentication and read endpoints remain public
- Auth: Register User, Login User, JWT access token generation, JWT bearer validation, and protected Current User endpoint implemented through API and persistence

Current source boundaries:

- API can call module Application layers.
- Infrastructure implements persistence and depends inward.
- Domain remains independent.
- BuildingBlocks must not reference modules.
- Catalog and Auth must not reference each other.

## Important Constraints

Do not create code without explicit `APPROVED: EXECUTE`.

Do not create these unless explicitly approved:

- Bootstrapper project
- Shared project
- new modules
- migrations
- database schema changes
- API endpoints
- Auth refresh-token, roles, permissions, protected Catalog read endpoint, or token persistence behavior
- Customers module

Prompt logging is required before execution unless the user writes `SKIP PROMPT LOG`.

When AGENT.md changes, replace the full file. Do not provide or apply partial AGENT.md edits.

AGENT.md is now a router. Detailed V2 rules live in `instructions/*.md`; read those files before planning or execution.

Reusable prompt defaults live in `docs/project/PROMPT_TEMPLATE.md`. Use it to expand short prompts such as `Plan next Catalog feature: Update Product Details` or `Execute approved feature: Update Product Details`.

Short planning prompts must follow the template's Plan Output Contract exactly and run the Plan Self-Validation Rule before returning a plan.

## Current Implementation Notes

Catalog product search uses an infrastructure read model:

- `ProductSearchReadModel`
- `CatalogReadDbContext`

This is documented in:

- `docs/decisions/ADR-001-product-search-read-model.md`

Do not revert product search back to aggregate value object access inside EF query filtering.

Catalog has one manual migration for the product schema. Do not add migrations without explicit approval.

Catalog supports Create Product, Get Product By Id, Search/List Products, Update Product Details, and Deactivate Product. Catalog `POST`, `PUT`, and `DELETE` product endpoints are protected; Catalog `GET` endpoints remain public. Update Product Details changes name and description only, preserves SKU, updates `UpdatedAt`, and does not require a migration.

Auth has a `User` aggregate with `UserId`, `Email`, and `PasswordHash` value objects. Register User is wired through `POST /api/auth/users/register`. Login User is wired through `POST /api/auth/users/login` and returns `userId`, `email`, `accessToken`, `tokenType`, and `expiresAt`. JWT bearer authentication is configured in the API with explicit default authenticate and challenge schemes, and `GET /api/auth/users/me` is protected with `[Authorize]` and returns `userId` and `email` from the token claims. Swagger uses the standard HTTP bearer scheme with per-operation security metadata for `[Authorize]` actions; enter the raw JWT access token in the Authorize field and Swagger UI sends `Authorization: Bearer {token}`. Swagger UI authorization persistence is enabled. If generated Swagger curl lacks the `Authorization` header, restart the running API process and refresh Swagger because stale Swagger JSON may be served. Avoid adding refresh tokens, roles, permissions, protected Catalog read endpoints, token persistence, or Customers integration until approved.

## Standard Verification

For execution tasks that touch code or project structure, run:

```text
dotnet restore Ecommerce.sln
dotnet build Ecommerce.sln
dotnet test Ecommerce.sln
```

For documentation-only tasks, do not run unnecessary build or test commands unless the user asks.

## Documentation Maintenance

After completed execution tasks, update:

- `docs/project/PROJECT_STATUS.md`
- `docs/project/AI_HANDOFF.md`
- `docs/project/ROADMAP.md`
- `docs/project/NEXT_SESSION.md`

Keep these files factual, concise, and aligned with the current repository.

Prompt logs under `docs/prompts/` are historical records. Do not rewrite old prompt logs for template cleanup or style normalization.

## Recent Completed Work

- Catalog Clean Architecture skeleton
- Architecture tests
- Create Product
- Get Product By Id
- Catalog database migration and LocalDB setup
- Swagger for local testing
- Search/List Products
- Update Product Details
- Global API error handling
- ValidationProblemDetails fix
- Product search EF translation fix
- Deactivate Product
- Product search read model ADR
- Auth module skeleton
- Auth User aggregate domain model
- Auth Register User Application layer
- Auth Register User persistence and API endpoint
- Auth Login User Application layer
- Auth Login User persistence and API endpoint
- Auth JWT access token generation
- Auth JWT bearer validation and protected Current User endpoint
- Swagger JWT authorization and protected Catalog write endpoints
- JWT default authentication scheme fix
- Swagger Authentication Integration
- Revisit Swagger/API Authentication Integration
- AI project memory documentation
- AGENT.md router and instruction file split
- Prompt standardization and reusable prompt template setup
- Prompt template compliance contract enhancement
