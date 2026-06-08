# Roadmap

## Completed

- Day 1 Clean Architecture solution skeleton
- Catalog module skeleton
- BuildingBlocks projects
- Architecture tests for dependency rules and project structure
- Catalog Create Product command
- Catalog Get Product By Id query
- Catalog SQL Server LocalDB setup
- Catalog initial EF Core migration
- Swagger/OpenAPI for local development
- Catalog Search/List Products query with pagination
- Global API error handling with ProblemDetails
- ValidationProblemDetails serialization fix
- EF-translatable product search over DDD value objects
- Catalog Deactivate Product command
- ADR for product search read model
- Auth module skeleton
- Project memory documentation
- AGENT.md router and instruction file split

## Current Priorities

- Keep architecture tests green as modules evolve.
- Keep project memory documentation current after execution tasks.
  - Update `docs/project/NEXT_SESSION.md` after every completed execution task.
  - Update `docs/project/PROJECT_STATUS.md` for state changes.
  - Update `docs/project/AI_HANDOFF.md` for constraint changes.
  - Update `docs/project/ROADMAP.md` for milestone changes.
- Keep AGENT.md and instruction files synchronized when operating rules change.
- Continue adding features only through explicit planning and approval.

## Catalog Candidates

Potential future work:

- Update Product
- Reactivate Product
- Product uniqueness hardening
- Integration tests for API and persistence
- Additional read models if query needs grow

Not currently started:

- price
- inventory
- category
- images
- variants
- SEO

## Auth Candidates

Auth is skeleton-only.

Potential future phases:

- User aggregate design
- Registration command
- Login command
- Password hashing strategy
- JWT issuing strategy
- Refresh token strategy
- Auth persistence model
- Auth API endpoints

Not currently started:

- User aggregate
- JWT
- refresh tokens
- password hashing
- Auth DbContext
- Auth migrations
- roles or permissions

## Platform Candidates

Potential future work:

- Integration testing setup
- API versioning decision
- Health checks
- Structured logging
- Configuration validation
- Authentication/authorization middleware after Auth design is approved

## Documentation Candidates

Potential future work:

- Additional ADRs for Auth token strategy
- Additional ADRs for module integration strategy
- Learning journal entries when requested
- README onboarding once the architecture stabilizes
