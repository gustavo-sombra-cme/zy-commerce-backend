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
- Backend Admin Product Management
- Catalog Reactivate Product
- Catalog Product Price Write Support
- Platform Health Checks
- Platform Structured Logging
- Orders Initial Vertical Slice
- Orders List For Current User
- ADR for Orders product snapshot strategy
- MCP Server Integration
- ADR for MCP server boundary and security
- Ecommerce Assistant Agent Phase 1
- ADR for assistant orchestration boundary and safety
- Assistant Intent Interpreter Phase 2
- ADR for untrusted assistant intent interpretation
- Assistant LLM Provider Integration Phase 3
- Backend Gemini LLM Provider for Ecommerce Assistant
- ADR for config-gated assistant LLM provider integration
- Assistant Text-to-SQL read-only database boundary
- Assistant Text-to-SQL SQL validator and read-only executor behind disabled feature flag
- Assistant Text-to-SQL LLM planner behind feature-flagged orchestration
- Assistant Text-to-SQL orchestration behind disabled feature flag with existing assistant fallback
- Conservative Assistant Text-to-SQL status cleanup
- Project memory documentation
- AGENT.md router and instruction file split
- Prompt standardization and reusable prompt template setup
- Prompt template compliance contract enhancement
- Feature demo slide deliverable workflow
- Backend branch workflow rules
- AI skills and sub-agent architecture documentation
- Phase 2A workflow skill wiring into backend Codex/harness instructions

## Current Priorities

- Keep architecture tests green as modules evolve.
- Use `docs/project/PROMPT_TEMPLATE.md` to keep future planning and execution prompts shorter while preserving approval, logging, architecture, DDD, CQRS, testing, and documentation rules.
- Use repo-local workflow skill docs under `docs/skills/workflow/` for repeated Codex/harness checks while keeping approval gates explicit.
- For short planning prompts, preserve the full Plan Output Contract and Plan Self-Validation Rule in `docs/project/PROMPT_TEMPLATE.md`.
- For main/demo-worthy feature executions, create or update `docs/demo/features/{feature-slug}-demo-slides.md` with slide-ready Markdown, Mermaid diagrams where useful, speaker cues, demo script, test evidence, risks/tradeoffs, and Q&A talking points.
- Keep project memory documentation current after execution tasks.
  - Update `docs/project/NEXT_SESSION.md` after every completed execution task.
  - Update `docs/project/PROJECT_STATUS.md` for state changes.
  - Update `docs/project/AI_HANDOFF.md` for constraint changes.
  - Update `docs/project/ROADMAP.md` for milestone changes.
- Keep AGENT.md and instruction files synchronized when operating rules change.
- Keep repo-local workflow skill and sub-agent guidance wired through `AGENT.md`, `instructions/*`, and `docs/project/PROMPT_TEMPLATE.md` without treating guidance as approval.
- Continue adding features only through explicit planning and approval.

## Catalog Candidates

Potential future work:

- Product uniqueness hardening
- Integration tests for API and persistence
- Additional read models if query needs grow

Not currently started:

- inventory
- category
- images
- variants
- SEO

## Auth Candidates

Auth currently has Register User and Login User implemented through API and persistence. Login returns a short-lived JWT access token with a `role` claim and no refresh token. JWT bearer validation is configured, Customer/Admin role persistence exists, and `GET /api/auth/users/me` is protected and returns the current user's id, email, and role from token claims.

Potential future phases:

- Refresh token strategy
- Admin user management strategy
- Broader permission model beyond Customer/Admin if needed

Not currently started:

- refresh tokens
- token persistence
- public admin registration endpoint

## Orders Candidates

Orders currently has Create Order, List Orders For Current User, and Get Order By Id. Orders stores product snapshot data from the create request, scopes reads to the authenticated buyer, and does not reference Catalog or Auth internals.

Potential future phases:

- Catalog validation/integration for product snapshots
- Inventory reservation
- Payments
- Shipping
- Order cancellation
- Refunds
- Advanced order status workflows
- Customer profile integration

Not currently started:

- payments
- inventory reservation
- shipping
- discounts or coupons
- cancellation
- refunds
- advanced status workflow
- Customer profile integration

## MCP Candidates

MCP currently exposes a protected API-layer allowlist for Catalog reads and initial Orders tools.

Potential future phases:

- Frontend MCP client integration
- Dedicated MCP authorization policies or scopes
- Rate limiting for `/mcp`
- Catalog snapshot validation before broader order creation usage
- Additional read-only Catalog tools after explicit approval

Not currently started:

- Auth MCP tools
- Catalog write MCP tools
- raw database or SQL MCP tools
- health readiness MCP tools
- MCP resources or prompts

## Assistant Candidates

Assistant exposes protected read-only Catalog/Orders orchestration through `POST /api/assistant/query`. It now uses an `IAssistantIntentInterpreter` abstraction with deterministic interpretation as disabled-mode default, config-gated LLM interpretation, selectable OpenAI-style or Gemini provider clients, strict untrusted `AssistantIntentPlan` validation, and fake provider tests only.

Runtime assistant sub-agents, if introduced later, should remain API-layer classes. Catalog, Orders, and Auth modules should not know about agents, prompts, model planning, Text-to-SQL routing, or provider diagnostics.

Potential future phases:

- Production provider configuration and operational smoke testing outside automated tests
- Gemini POC demo validation against account/project-specific free-tier and rate-limit behavior
- Additional provider-specific payload tuning if future providers are introduced
- Additional read-only analysis tools after explicit approval
- Text-to-SQL operational smoke testing after local read-only connection strings are configured
- Selectable Text-to-SQL strategy with explicit telemetry, without moving or converting the current Text-to-SQL implementation into a skill until separately approved
- Dedicated assistant authorization policy or rate limiting
- Frontend integration with the backend assistant endpoint

Not currently started:

- mutating assistant actions
- order creation through assistant
- Catalog writes through assistant
- raw SQL or database tools
- admin analytics
- cross-user analysis
- committed LLM API keys or secrets
- provider SDK packages
- live provider calls in automated tests

## Platform Candidates

Potential future work:

- Integration testing setup
- API versioning decision
- Configuration validation
- Broader authorization policies after Auth design is approved

## Documentation Candidates

Potential future work:

- Additional ADRs for Auth token strategy
- Additional ADRs for module integration strategy
- Learning journal entries when requested
- README onboarding once the architecture stabilizes
- Reusable cross-project Codex skill for feature demo slide generation after the repo workflow proves stable
- Additional repo-local workflow skill examples if future backend workflow gaps appear
- Later consider API-layer runtime assistant sub-agents through a separate approved plan; do not infer them from the workflow Markdown guidance.
