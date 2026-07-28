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

- Catalog: implemented product features; write endpoints require Admin bearer authorization and read endpoints remain public
- Auth: Register User, Login User, JWT access token generation with role claims, JWT bearer validation, Customer/Admin role persistence, and protected Current User endpoint implemented through API and persistence
- Orders: Create Order, List Orders For Current User, and Get Order By Id implemented with product snapshots and owner-scoped reads
- Platform/API: health checks expose process liveness and Auth/Catalog/Orders database readiness; structured logging uses `X-Correlation-ID`; MCP exposes a protected allowlisted tool surface; the assistant endpoint exposes read-only Catalog/Orders orchestration with deterministic default intent interpretation, config-gated LLM interpretation, and validated untrusted intent plans

Current source boundaries:

- API can call module Application layers.
- Infrastructure implements persistence and depends inward.
- Domain remains independent.
- BuildingBlocks must not reference modules.
- Catalog, Auth, and Orders must not reference each other.
- MCP is API-layer only and must dispatch existing Application/CQRS requests through `ISender`.
- MCP adapter code must not call EF Core DbContexts, repositories, Domain objects, or module internals directly.
- Assistant orchestration is API-layer only and must dispatch existing read-side Application/CQRS requests through `ISender`.
- Assistant code must not call EF Core DbContexts, repositories, Domain objects, module internals, write commands, or MCP protocol packages directly.

## Important Constraints

Do not create code without explicit `APPROVED: EXECUTE`.

Every approved execution task must follow the Branch Workflow Rules in `instructions/01-execution-and-planning.md`: start from latest `main`, confirm path/branch/status, stop on a dirty worktree unless explicitly approved, create a dedicated `feature/`, `fix/`, `docs/`, or `chore/` branch before implementation, and do not work directly on `main`.

Do not create these unless explicitly approved:

- Bootstrapper project
- Shared project
- new modules
- migrations
- database schema changes
- API endpoints
- Auth refresh-token, protected Catalog read endpoint, token persistence behavior, public admin registration endpoint, or broader permission model
- Customers module

Prompt logging is required before repository planning, approved execution, artifact-producing testing, documentation changes, Skill maintenance, findings-first repository review, and global workflow-policy work unless the user writes `SKIP PROMPT LOG`. General explanation and read-only questions that create no repository artifact do not require a prompt log.

Commit, push, and pull request creation are manual gates. Do not commit, push, or create a PR unless the user explicitly approves one of `APPROVED: COMMIT BACKEND CHANGES`, `APPROVED: PUSH`, `APPROVED: PUSH BACKEND BRANCH`, `APPROVED: CREATE BACKEND PR`, or `APPROVED: COMMIT AND PUSH BACKEND CHANGES`.

When AGENT.md changes, replace the full file. Do not provide or apply partial AGENT.md edits.

AGENT.md is now a router. Detailed V2 rules live in `instructions/*.md`; read those files before planning or execution.

Reusable prompt defaults live in `docs/project/PROMPT_TEMPLATE.md`. Use it to expand short prompts such as `Plan next Catalog feature: Update Product Details` or `Execute approved feature: Update Product Details`.

Short planning prompts must follow the template's Plan Output Contract exactly and run the Plan Self-Validation Rule before returning a plan.

Repository-local workflow Skills are valid Codex Skill packages under `.agents/skills/`; workflow sub-agent guidance remains under `docs/agents/workflow/`. They are backend-specific, not shared with the frontend repository, and do not replace explicit user approval.

Readiness Skills compose evidence: commit readiness requires current branch/scope plus passing executed verification and secret scan, with applicable review, migration, prompt-log, and project-memory evidence; push readiness additionally requires passing commit-readiness, executed-verification, and secret-scan results. Verification dry runs classify checks and never claim `VERIFICATION_STATUS: PASS`. Credential-only concerns route to secret scanning, not migration safety, unless database target, ownership, permissions, schema, migration, execution, or SQL behavior changes.

Current wiring connects those Skills and sub-agent docs to the backend Codex/harness instructions and prompt template only. It does not change runtime assistant behavior, Text-to-SQL behavior, MCP behavior, frontend contracts, database schema, CI, project files, or appsettings secrets.

Project-wide AI skills and sub-agent decisions live in `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md`. Catalog and Orders runtime assistant sub-agents are API-layer classes and dispatch business reads through existing Application/CQRS handlers; future runtime sub-agents must preserve that boundary unless a separately approved architecture change says otherwise.

Phase 3A introduced the first runtime API-layer assistant sub-agent: order-specific assistant CQRS orchestration now lives behind `IOrdersAssistantSubAgent`/`OrdersAssistantSubAgent`. `AssistantOrchestrator` still owns Text-to-SQL first-pass/fallback, intent interpretation, catalog handling, unsupported responses, and response contracts. Text-to-SQL was not moved, converted into a skill, or changed into a selectable strategy in this task.

Phase 3B introduced the API-layer `ICatalogAssistantSubAgent`/`CatalogAssistantSubAgent` for existing catalog assistant CQRS orchestration. `AssistantOrchestrator` still owns Text-to-SQL first-pass/fallback, intent interpretation, order delegation, catalog delegation, unsupported responses, and response contracts. Text-to-SQL and `OrdersAssistantSubAgent` were not changed, and broad product search by text/name/SKU was not added.

Phase 3C reviewed the post-extraction `AssistantOrchestrator`. It remains the high-level coordinator for top-level query flow, Text-to-SQL first-pass/fallback, intent interpretation, validation, diagnostics, Orders/Catalog sub-agent delegation, and final unsupported fallback. Do not add `SupportAssistantSubAgent` or `SafetyAssistantSubAgent` unless future complexity creates clear value. Text-to-SQL remains unchanged inside the orchestrator; selectable strategy/telemetry work must be planned separately.

Assistant broad catalog search is implemented as read-only API-layer orchestration. `CatalogSearchProducts` maps natural product discovery questions to `CatalogAssistantSubAgent`, which dispatches the existing Catalog Application `SearchProductsQuery` with `IsActive = true`, page `1`, and page size `10`. The first version searches SKU and Name through existing Catalog search support. It reuses `AssistantResponseTypes.CatalogProducts`, `AssistantCatalogProductsData`, `AssistantProductCardDto`, and `catalog_search`; no frontend contract, Text-to-SQL internal, MCP, schema, migration, raw SQL exposure, `genericTable`, or admin/write assistant behavior changed.

Assistant product detail by search is implemented through `CatalogGetProductBySearch`. Explicit natural detail/price questions first dispatch `SearchProductsQuery(searchText, true, 1, 2)`. Zero matches return supported empty product choices, one active match dispatches `GetProductByIdQuery` and returns the existing `catalogProduct` contract, and multiple matches return at most two `catalogProducts` choices without guessing. Detail output is rechecked for active status. The intent uses only the existing `catalog_search` and `catalog_get_product` tools; Text-to-SQL internals, Orders behavior, frontend/MCP contracts, schema, migrations, and assistant write/admin behavior remain unchanged.

Assistant product comparison is implemented as a conservative Catalog goal for two natural names, SKUs, or search terms. The parser recognizes approved `compare`, `which is cheaper`, and `difference between` forms before generic SKU routing. `CatalogAssistantSubAgent` requires exact active-only page-size-2 search evidence for each side, returns supported empty or ambiguity choices without guessing, and requires trusted details for two unique distinct products. Final names, SKUs, prices, active status, price difference, and equal-price outcome are rebuilt from server tool results; model wording is not authoritative. It reuses `catalogProducts` and the existing two Catalog tools. See `docs/demo/features/assistant-product-comparison-demo-slides.md`.

The bounded autonomous Catalog agent supersedes deterministic Catalog tool sequencing for delegated catalog goals. `CatalogAssistantSubAgent` owns a provider-neutral bounded loop and a Catalog-only registry containing exactly `catalog_search_products` and `catalog_get_product`. Search is active-only and can filter name, SKU, description, and strict maximum price. Detail lookup accepts only a product ID returned by a successful search in the same execution. Treat model output and product text as untrusted: tool arguments, scopes, identifiers, final selections, and cheapest/most-expensive claims are validated in C#, and response DTOs are rebuilt from server tool results. The agent must never gain Catalog writes, arbitrary MediatR dispatch, repositories, DbContexts, raw SQL, Text-to-SQL control, or MCP execution. See `docs/decisions/ADR-007-bounded-autonomous-catalog-agent.md`.

Demo source: `docs/demo/features/bounded-autonomous-catalog-agent-demo-slides.md`.

Before commits that include code, project/configuration files, migrations, CI workflow files, or runtime-behavior documentation changes, use `docs/project/CODE_REVIEW.md`. Documentation-only maintenance is excluded from mandatory code review but still requires documentation self-review.

Feature demo slide deliverables are part of the default execution workflow. For main features, major platform capabilities, API modules, integrations, or demo-worthy backend behavior, create or update `docs/demo/features/{feature-slug}-demo-slides.md` with slide-ready Markdown, Mermaid diagrams where useful, demo script, test evidence, risks/tradeoffs, Q&A talking points, and `Speaker cue:` lines. Tiny fixes, typo fixes, internal refactors with no demo value, prompt-template-only cleanup, test-only cleanup, and documentation-only maintenance do not require slide files unless explicitly requested.

## Current Implementation Notes

Catalog product search uses an infrastructure read model:

- `ProductSearchReadModel`
- `CatalogReadDbContext`

This is documented in:

- `docs/decisions/ADR-001-product-search-read-model.md`

Do not revert product search back to aggregate value object access inside EF query filtering.

Catalog has two manual migrations for the product schema: `InitialCatalogSchema` and `AddProductPrice`. Do not add migrations without explicit approval.

Catalog supports Create Product, Get Product By Id, Search/List Products, Update Product Details, Update Product Price, Deactivate Product, and Reactivate Product. Create Product accepts non-negative `price`, stores it on the Product aggregate with `decimal(18,2)` persistence, and Catalog search/details responses return `price`. Catalog write endpoints require the API `RequireAdmin` policy; Catalog `GET` endpoints remain public. Update Product Details changes name and description only, preserves SKU and price, and updates `UpdatedAt`. Update Product Price changes price only, preserves historical Orders snapshots, and updates `UpdatedAt`. Reactivate Product is idempotent: an inactive product becomes active and updates `UpdatedAt`; an already active product returns success without changing state or updating `UpdatedAt`.

Auth has a `User` aggregate with `UserId`, `Email`, `PasswordHash`, and `UserRole`. Register User is wired through `POST /api/auth/users/register` and defaults new users to `Customer`. Login User is wired through `POST /api/auth/users/login` and returns `userId`, `email`, `accessToken`, `tokenType`, and `expiresAt`; the JWT includes a `role` claim. JWT bearer authentication is configured in the API with explicit default authenticate/challenge schemes and `RoleClaimType = "role"`. `GET /api/auth/users/me` is protected with `[Authorize]` and returns `userId`, `email`, and `role` from token claims. Swagger uses the standard HTTP bearer scheme with per-operation security metadata for `[Authorize]` actions; enter the raw JWT access token in the Authorize field and Swagger UI sends `Authorization: Bearer {token}`. Swagger UI authorization persistence is enabled. If generated Swagger curl lacks the `Authorization` header, restart the running API process and refresh Swagger because stale Swagger JSON may be served. Avoid adding refresh tokens, token persistence, public admin registration, protected Catalog read endpoints, or Customers integration until approved.

Platform health checks are API-only:

- `GET /health/live` checks process liveness only and must not depend on database connectivity.
- `GET /health/ready` checks Auth, Catalog, and Orders database readiness through existing EF Core DbContexts using `Database.CanConnectAsync`.
- Health readiness must not create databases, apply migrations, or change schema.
- No package was added for health checks.

Platform structured logging is API-only:

- Uses built-in ASP.NET Core logging only.
- `X-Correlation-ID` is accepted, preserved when supplied, generated when missing, and returned on every response.
- Request, exception, health readiness failure, and JWT authentication failure/challenge logs are structured.
- Never log tokens, authorization headers, passwords, request bodies, or response bodies.
- No package, migration, schema, Domain, Application, CQRS, module, or business behavior change was added for structured logging.

Platform MCP integration is API-only:

- Uses the official `ModelContextProtocol.AspNetCore` package in `Ecommerce.Api`.
- `POST /mcp` is hosted as a protected stateless Streamable HTTP MCP endpoint.
- MCP implementation lives under `src/Api/Ecommerce.Api/Mcp`.
- MCP tools call existing Application/CQRS requests through `ISender`.
- Approved tool allowlist:
  - `catalog_search_products`
  - `catalog_get_product_by_id`
  - `orders_get_order_by_id`
  - `orders_create_order`
- `orders_create_order` requires explicit `confirmedByUser` input.
- Orders MCP tools use the authenticated JWT `sub` claim for buyer/user context.
- MCP must not expose Auth register/login, JWTs, passwords, authorization headers, raw database access, migrations, health readiness details, appsettings, environment variables, SQL, Catalog writes, cross-user orders, or non-existent Orders features.
- ADR-003 documents MCP boundary and security.

Platform assistant orchestration is API-only:

- `POST /api/assistant/query` is protected by bearer authentication.
- Request body accepts only a natural-language `question`; it does not accept `userId`, `buyerId`, or tool selection from the caller.
- Implementation lives under `src/Api/Ecommerce.Api/Assistant` and `src/Api/Ecommerce.Api/Controllers/Assistant`.
- The assistant uses `IAssistantIntentInterpreter`; disabled-mode production DI resolves `DeterministicAssistantIntentInterpreter` by default.
- `LlmAssistantIntentInterpreter` is available behind `Assistant:Llm:Enabled` and uses `IAssistantLlmClient`, `HttpClientFactory`, and `System.Text.Json`.
- Provider selection is backend configuration only. `Assistant:Llm:Provider` defaults to `OpenAI`; `ECOMMERCE_ASSISTANT_LLM_PROVIDER=Gemini` overrides it and selects `GeminiAssistantLlmClient`.
- Gemini is a POC/testing provider. Configure it with `ECOMMERCE_ASSISTANT_GEMINI_API_KEY`, optional `ECOMMERCE_ASSISTANT_GEMINI_MODEL` (default `gemini-2.5-flash`), and optional `ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT` (default `https://generativelanguage.googleapis.com/v1beta`).
- Gemini free-tier and rate-limit behavior varies by Google account/project. A ChatGPT/OpenAI subscription is unrelated to Gemini Developer API access.
- The provider adapter returns only structured `AssistantIntentPlan` JSON and never executes tools directly.
- Interpreter output is represented as an untrusted `AssistantIntentPlan` and must pass `AssistantIntentPlanValidator` before any execution.
- The validator rejects unknown tools, invalid arguments, unsafe questions, mutating/admin/SQL/cross-user plans, and model-provided `userId`/`buyerId` scope.
- No provider SDK package, committed API key, committed secret, live test call, runtime database access, or MCP dependency has been added for LLM provider execution.
- API keys must come only from environment variables or user secrets/non-committed configuration providers. Do not put API keys in `appsettings*.json`.
- Assistant Text-to-SQL Task 1 added the future database boundary: the `assistant` schema and approved read-only views in the separate Catalog and Orders databases, plus setup documentation in `docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md`.
- Assistant Text-to-SQL Task 2 added the SQL validator and read-only executor behind `Assistant:TextToSql:Enabled`, which defaults to `false`.
- Assistant Text-to-SQL Task 3 added the LLM planner under `src/Api/Ecommerce.Api/Assistant/TextToSql`. It builds the approved-view prompt, parses the model JSON plan fail-closed, and reuses the existing assistant LLM client abstraction.
- Assistant Text-to-SQL Task 4 wired `AssistantOrchestrator` to try Text-to-SQL as a first-pass path only when `Assistant:TextToSql:Enabled` is true. When the flag is false, the existing assistant flow remains unchanged.
- Existing deterministic/intent assistant behavior remains the fallback when planning, validation, execution, or response mapping fails safely.
- Task 5A only cleaned stale documentation/status/test naming; it did not remove deterministic fallback, the existing CQRS assistant flow, response DTOs, tool names, or feature-flag behavior.
- Text-to-SQL candidate SQL is still untrusted and must pass the Task 2 validator before execution.
- Text-to-SQL runtime must use separate local-only read-only connection strings: `ConnectionStrings:AssistantCatalogReadOnly` and `ConnectionStrings:AssistantOrdersReadOnly`. Do not commit real values or passwords.
- Do not use normal application DB connection strings for Text-to-SQL execution.
- Generated SQL is never returned to the frontend and `genericTable` is not exposed publicly.
- The assistant read-only SQL principals must be granted `SELECT` only on the `assistant` schema/views in their owning databases and no direct access to base Catalog, Orders, or Auth tables.
- The assistant views expose Catalog product fields and owner-scoped Orders fields with `BuyerUserId`; they do not expose `auth.Users`, password hashes, tokens, or auth internals.
- Do not log prompts, raw provider responses, API keys, tokens, auth headers, full Gemini request URIs, or sensitive payloads.
- Temporary assistant LLM configuration diagnostics log only booleans/presence flags and fallback/failure status; they must not be expanded to include prompts, raw provider responses, API key values, auth headers, tokens, or sensitive payloads.
- Approved internal capability allowlist: `catalog_search`, `catalog_get_product`, `orders_search`, `orders_get_order`, `orders_analyze`.
- Orders analysis uses the authenticated JWT `sub` claim for buyer scope.
- Supported Phase 1 questions include recent orders, products ordered, orders containing a product/SKU/name, total spend, most frequently purchased products, products under an amount, and orders containing products over an amount.
- Mutating, admin, SQL, token, database, internal, unclear, and cross-user requests must return safe unsupported responses.
- ADR-004 documents the assistant orchestration boundary and safety model.
- ADR-005 documents untrusted assistant intent interpretation and plan validation.
- ADR-006 documents config-gated LLM provider integration.

Orders current user order flows are implemented:

- Projects exist: Domain, Application, Infrastructure, Contracts, UnitTests.
- Domain model exists: `Order`, `OrderLine`, `OrderId`, `OrderLineId`, `BuyerId`, and `OrderStatus`.
- API endpoints exist: `POST /api/orders`, `GET /api/orders`, and `GET /api/orders/{orderId}`.
- All Orders endpoints require bearer authentication.
- `GET /api/orders` is scoped to the authenticated buyer id, returns summaries only, supports `pageNumber`/`pageSize`, defaults to `1`/`20`, caps `pageSize` at `100`, and sorts newest first by `CreatedAt` descending.
- `GET /api/orders/{orderId}` is scoped to the authenticated buyer id and returns not found for orders not owned by the user.
- Orders uses product snapshot data supplied in the create request for this slice.
- Product snapshot strategy is documented in `docs/decisions/ADR-002-orders-product-snapshot-strategy.md`.
- Orders has one manual migration: `20260612090403_InitialOrdersSchema.cs`.
- Local `EcommerceOrders` database was created and updated through `InitialOrdersSchema`.
- Do not add direct Orders references to Catalog or Auth internals.
- Do not add payments, inventory reservation, shipping, discounts, coupons, cancellation, refunds, advanced order workflows, Customer profile integration, or additional Orders MCP tools beyond the approved allowlist without explicit approval.

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

For main/demo-worthy feature executions, also create or update the related feature demo slide source under `docs/demo/features/` and mention the path in the execution summary. Project memory should reference the slide file when the feature changes project state.

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
- Catalog Reactivate Product
- Platform Health Checks
- Platform Structured Logging
- Orders Initial Vertical Slice
- Orders List For Current User
- Catalog Product Price Write Support
- Ecommerce Assistant Agent Phase 1
- Assistant Intent Interpreter Phase 2
- Assistant LLM Provider Integration Phase 3
- Assistant LLM Configuration Diagnostics
- Backend Admin Product Management
- MCP Server Integration
- AI project memory documentation
- AGENT.md router and instruction file split
- Prompt standardization and reusable prompt template setup
- Prompt template compliance contract enhancement
- Feature demo slide deliverable workflow
- Backend branch workflow rules
- AI skills and sub-agent architecture documentation
- Ecommerce Skill normalization, routing defect repair, and focused conditional-loading regression coverage
- Phase 3A runtime API-layer Orders assistant sub-agent extraction
- Phase 3B runtime API-layer Catalog assistant sub-agent extraction
- Phase 3C Assistant Orchestrator cleanup review
- Assistant broad catalog search
- Assistant product detail by natural name or SKU
- Assistant product comparison by name or SKU
