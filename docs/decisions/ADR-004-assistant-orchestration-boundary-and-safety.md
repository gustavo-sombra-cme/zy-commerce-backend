# ADR-004: Assistant Orchestration Boundary And Safety

## Date

2026-06-19

## Status

Accepted

## Context

The backend already exposes REST APIs and a protected MCP endpoint for selected e-commerce capabilities. The frontend assistant and MCP surface can call direct tools, but product questions such as "What products did I order?" or "What is my total spend?" require backend-side orchestration over multiple safe reads.

The repository follows Clean Architecture, CQRS, module isolation, and thin API adapter rules. Orders data must be scoped to the authenticated buyer, and Catalog/Orders modules must not be coupled through internals.

## Options Considered

1. Add an external LLM provider immediately.

   Rejected for this phase because it would require package/config/secret choices, prompt-injection hardening, provider governance, cost controls, and observability decisions.

2. Let the assistant query EF Core DbContexts or repositories directly.

   Rejected because it bypasses Application/CQRS boundaries and turns the API adapter into a persistence-aware component.

3. Reuse MCP protocol types internally.

   Rejected because the REST assistant should not depend on MCP protocol concerns or tool schemas.

4. Add a deterministic API-layer orchestrator that dispatches existing read-side CQRS queries through `ISender`.

   Accepted.

## Decision

Implement `POST /api/assistant/query` as a protected API-layer endpoint.

The assistant is deterministic and read-only in Phase 1. It uses an internal allowlist of capabilities:

- `catalog_search`
- `catalog_get_product`
- `orders_search`
- `orders_get_order`
- `orders_analyze`

Orders analysis always uses the authenticated JWT `sub` claim as buyer id. The assistant does not accept user id or buyer id from the request body.

The assistant must not:

- dispatch write commands
- call EF Core DbContexts directly
- call repositories directly
- call Domain objects directly
- use raw SQL
- expose cross-user data
- expose tokens, SQL, exception details, or internal routing prompts
- call external AI providers in this phase

## Rationale

This keeps natural-language orchestration at the API/platform boundary while preserving module ownership. Catalog and Orders continue to expose behavior through Application queries, and the assistant composes those queries without creating new business state or new persistence paths.

The deterministic first slice is intentionally modest: predictable routing, explicit unsupported responses, and high testability are more important than broad language coverage.

## Consequences

Positive:

- Adds a usable backend assistant endpoint without external AI dependencies.
- Preserves Clean Architecture and CQRS boundaries.
- Keeps Orders data owner-scoped.
- Makes unsafe requests explicitly unsupported.

Tradeoffs:

- Natural-language coverage is limited to supported patterns.
- Orders line analysis may require loading details for a bounded set of owned orders.
- Future richer analytics may require dedicated read queries.

## Risks

- Ambiguous user questions may route incorrectly unless unsupported handling stays conservative.
- Loading many order details can become inefficient.
- Future LLM integration could weaken boundaries if not governed by the same allowlist model.
