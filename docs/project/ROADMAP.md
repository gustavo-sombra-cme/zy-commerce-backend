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
- Catalog Update Product Details command
- Global API error handling with ProblemDetails
- ValidationProblemDetails serialization fix
- EF-translatable product search over DDD value objects
- Catalog Deactivate Product command
- ADR for product search read model
- Auth module skeleton
- Auth User aggregate domain model
- Auth Register User Application layer
- Auth Register User persistence and API endpoint
- Auth Login User Application layer
- Auth Login User persistence and API endpoint
- Auth JWT access token generation
- Auth JWT bearer validation and protected Current User endpoint
- Swagger JWT authorization and protected Catalog write endpoints
- Swagger Authorization header fix
- Swagger Authentication Integration
- Revisit Swagger/API Authentication Integration
- JWT default authentication scheme fix
- Project memory documentation
- AGENT.md router and instruction file split
- Prompt standardization and reusable prompt template setup
- Prompt template compliance contract enhancement

## Current Priorities

- Keep architecture tests green as modules evolve.
- Use `docs/project/PROMPT_TEMPLATE.md` to keep future planning and execution prompts shorter while preserving approval, logging, architecture, DDD, CQRS, testing, and documentation rules.
- For short planning prompts, preserve the full Plan Output Contract and Plan Self-Validation Rule in `docs/project/PROMPT_TEMPLATE.md`.
- Keep project memory documentation current after execution tasks.
  - Update `docs/project/NEXT_SESSION.md` after every completed execution task.
  - Update `docs/project/PROJECT_STATUS.md` for state changes.
  - Update `docs/project/AI_HANDOFF.md` for constraint changes.
  - Update `docs/project/ROADMAP.md` for milestone changes.
- Keep AGENT.md and instruction files synchronized when operating rules change.
- Continue adding features only through explicit planning and approval.

## Catalog Candidates

Potential future work:

- Reactivate Product
- Product uniqueness hardening
- Integration tests for API and persistence
- Authorization policy refinement for Catalog writes
- Additional read models if query needs grow

Not currently started:

- price
- inventory
- category
- images
- variants
- SEO

## Auth Candidates

Auth currently has Register User and Login User implemented through API and persistence. Login returns a short-lived JWT access token and no refresh token. JWT bearer validation is configured, and `GET /api/auth/users/me` is protected and returns the current user's id and email from token claims.

Potential future phases:

- Refresh token strategy
- Protected endpoint authorization policy strategy beyond the current Auth `/me` endpoint

Not currently started:

- refresh tokens
- roles or permissions
- token persistence

## Platform Candidates

Potential future work:

- Integration testing setup
- API versioning decision
- Health checks
- Structured logging
- Configuration validation
- Broader authorization policies after Auth design is approved

## Documentation Candidates

Potential future work:

- Additional ADRs for Auth token strategy
- Additional ADRs for module integration strategy
- Learning journal entries when requested
- README onboarding once the architecture stabilizes
