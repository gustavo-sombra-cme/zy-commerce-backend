# ADR-003: MCP Server Boundary And Security

## Date

2026-06-12

## Status

Accepted

## Context

Catalog, Auth, Orders, health checks, and structured logging are implemented. The next platform feature exposes selected e-commerce capabilities through an MCP server.

The repository is a Clean Architecture modular monolith. Modules must remain isolated, controllers must stay thin, and module internals must not be called directly from platform adapters. Existing Catalog and Orders use cases already exist as Application-layer CQRS requests through MediatR.

MCP introduces a model-facing protocol surface. That surface can be useful for AI-assisted workflows, but it also increases the risk of overexposure, unsafe tool invocation, cross-user data access, and accidental disclosure of tokens or operational details.

## Options Considered

1. Host MCP in a separate process that calls the REST API.

   Rejected for the initial implementation because it adds deployment complexity, duplicates API error mapping, introduces token-forwarding concerns, and makes local development heavier than needed.

2. Host MCP inside `Ecommerce.Api` and call EF Core DbContexts or repositories directly.

   Rejected because it bypasses Application use cases, weakens CQRS boundaries, and creates a transport adapter that knows too much about persistence.

3. Host MCP inside `Ecommerce.Api` and call existing Application/CQRS requests through `ISender`.

   Accepted.

4. Expose all API capabilities automatically as MCP tools.

   Rejected because MCP tools are model-invokable and must be explicitly allowlisted.

5. Expose only a small initial tool allowlist.

   Accepted.

## Decision

MCP is implemented as an API-layer adapter under `Ecommerce.Api`.

The API hosts protected `/mcp` using the official `ModelContextProtocol.AspNetCore` package and stateless Streamable HTTP transport. MCP tools dispatch existing Catalog and Orders Application/CQRS requests through `ISender`.

The initial MCP allowlist is:

- `catalog_search_products`
- `catalog_get_product_by_id`
- `orders_get_order_by_id`
- `orders_create_order`

`orders_create_order` requires explicit `confirmedByUser` input before dispatching the create command.

The MCP adapter must not expose:

- Auth register/login
- JWTs
- passwords
- authorization headers
- raw database access
- migrations
- health readiness details
- appsettings
- environment variables
- SQL
- Catalog writes
- cross-user orders
- non-existent Orders features

## Rationale

Keeping MCP in the API layer matches the existing thin-controller pattern: transport adapters receive requests, extract transport/user context, dispatch Application requests, and map safe responses.

Calling Application/CQRS requests through `ISender` preserves Clean Architecture and lets Catalog and Orders continue to own their business behavior. The MCP adapter does not become a second persistence or domain execution path.

Using an allowlist is safer than automatic exposure because MCP clients may be driven by model output. Each exposed tool must be intentionally reviewed for authorization, side effects, data shape, and logging risk.

Stateless Streamable HTTP keeps the first implementation simple and aligns with the existing bearer-token authentication model.

## Consequences

Positive:

- MCP exposure is isolated to `Ecommerce.Api`.
- Domain, Application, Infrastructure, and module behavior remain unchanged.
- Existing CQRS handlers remain the only execution path for Catalog and Orders behavior.
- `/mcp` requires bearer authentication.
- Orders reads remain owner-scoped through `GetOrderByIdQuery`.
- Order creation requires explicit confirmation input.

Tradeoffs:

- MCP introduces one approved package dependency in the API project.
- MCP tool schemas must be maintained alongside Application contracts.
- The Orders create tool still inherits the initial product snapshot spoofing risk from ADR-002 until Catalog validation is approved.
- MCP is protected by existing bearer authentication but does not yet have dedicated scopes, roles, rate limits, or OAuth resource metadata.

## Risks

- Overexposing new tools without allowlist review.
- Accidentally logging MCP arguments or results.
- Future tools bypassing Application/CQRS and calling persistence directly.
- Cross-user Orders access if buyer id is not consistently taken from the authenticated principal.
- Prompt-injection pressure to call write tools without user confirmation.
- Frontend MCP clients using overly broad bearer tokens.
