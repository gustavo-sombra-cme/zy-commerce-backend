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
9. `docs/project/PROJECT_STATUS.md`
10. `docs/project/ROADMAP.md`
11. Latest files in `docs/prompts/`
12. Relevant ADRs in `docs/decisions/`

## Current Shape

The backend is a Clean Architecture modular monolith on .NET 9.

Current modules:

- Catalog: implemented product features
- Auth: skeleton only

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
- Auth business behavior
- Customers module

Prompt logging is required before execution unless the user writes `SKIP PROMPT LOG`.

When AGENT.md changes, replace the full file. Do not provide or apply partial AGENT.md edits.

AGENT.md is now a router. Detailed V2 rules live in `instructions/*.md`; read those files before planning or execution.

## Current Implementation Notes

Catalog product search uses an infrastructure read model:

- `ProductSearchReadModel`
- `CatalogReadDbContext`

This is documented in:

- `docs/decisions/ADR-001-product-search-read-model.md`

Do not revert product search back to aggregate value object access inside EF query filtering.

Catalog has one manual migration for the product schema. Do not add migrations without explicit approval.

Auth projects are intentionally empty skeletons. Avoid adding User, JWT, password hashing, DbContext, endpoints, roles, or permissions until approved.

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

Keep these files factual, concise, and aligned with the current repository.

## Recent Completed Work

- Catalog Clean Architecture skeleton
- Architecture tests
- Create Product
- Get Product By Id
- Catalog database migration and LocalDB setup
- Swagger for local testing
- Search/List Products
- Global API error handling
- ValidationProblemDetails fix
- Product search EF translation fix
- Deactivate Product
- Product search read model ADR
- Auth module skeleton
- AI project memory documentation
- AGENT.md router and instruction file split
