# Prompt 073: Ecommerce Assistant Agent Phase 1 Execution

## Prompt Number

073

## Date

2026-06-19

## Purpose

Execute the approved Ecommerce Assistant Agent Phase 1 plan.

## Full Prompt

APPROVED: EXECUTE Ecommerce Assistant Agent Phase 1

Explicit approval:
- Add new protected endpoint: POST /api/assistant/query

Scope:
- Backend only.
- API / Platform Orchestration Feature.
- Deterministic rule-based assistant only.
- No external LLM provider.
- No new packages unless absolutely required.
- No database changes.
- No migrations.
- No MCP protocol dependency.
- Read-only only.

Implement:
- AssistantController
- AssistantQueryRequest
- AssistantQueryResponse
- AssistantOrchestrator
- AssistantIntentRouter
- AssistantToolRegistry
- AssistantToolNames / capability names
- Safe unsupported response handling
- DI registration

Supported capabilities:
- catalog_search
- catalog_get_product
- orders_search
- orders_get_order
- orders_analyze

Supported question types:
- Show my recent orders
- What products did I order?
- Which orders contain product/SKU/name 4444?
- What is my total spend?
- What did I buy most often?
- Find products under 20
- Find orders containing products over 10

Rules:
- Use ISender to dispatch existing Catalog/Orders read-side CQRS queries.
- Do not call EF DbContexts directly.
- Do not call repositories directly.
- Do not call Domain objects directly.
- Do not dispatch write commands.
- Do not expose cross-user data.
- Do not accept userId/buyerId from request body.
- Use authenticated JWT sub for Orders scope.
- Return unsupported for mutating/admin/SQL/token/database/internal requests.

Tests:
- Authorization
- Tool allowlist
- Intent routing
- Owner scoping
- Unsafe request handling
- Safe response shape

Documentation:
- Add prompt logs.
- Add ADR-004: Assistant Orchestration Boundary And Safety.
- Update project memory docs.
- Update product/API docs if applicable.

Verification:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

## Status

EXECUTED

## Result Summary

Implemented Ecommerce Assistant Agent Phase 1 as a protected deterministic API-layer endpoint at `POST /api/assistant/query`.

Created:

- Assistant controller, request/response contracts, orchestrator, intent router, tool registry, and capability names.
- Internal read-only capability allowlist for `catalog_search`, `catalog_get_product`, `orders_search`, `orders_get_order`, and `orders_analyze`.
- Safe unsupported handling for mutating, admin, SQL, token, database, internal, unclear, and cross-user requests.
- Authorization, tool allowlist, routing, owner-scoping, unsafe request, safe response, and boundary tests.
- ADR-004 for assistant orchestration boundary and safety.
- Project memory and product/API documentation updates.

Verification:

- `dotnet restore Ecommerce.sln`: passed.
- `dotnet build Ecommerce.sln`: blocked by running process `Ecommerce.Api (36008)` locking the normal API `bin` output.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\assistant-verify`: passed.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\assistant-test`: passed.
- Catalog unit tests: 75 passed.
- Auth unit tests: 65 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 71 passed.

No external LLM provider, packages, database changes, migrations, MCP changes, Catalog writes, Orders writes, Auth behavior changes, or frontend behavior changes were added.
