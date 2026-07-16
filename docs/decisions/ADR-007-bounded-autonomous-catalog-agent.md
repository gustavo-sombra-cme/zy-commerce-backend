# ADR-007: Bounded Autonomous Catalog Agent

## Date

2026-07-16

## Status

Accepted

## Context

The API-layer `CatalogAssistantSubAgent` is currently a deterministic handler for catalog-specific intents. It selects a predefined method and query sequence. Rich catalog goals such as selecting the cheapest matching product and then retrieving details require bounded multi-step reasoning while preserving the assistant safety boundary established by ADR-004, ADR-005, and ADR-006.

## Options Considered

1. Add more deterministic intent branches.

   Rejected because multi-step selection would continue to centralize catalog sequencing in the main intent system and would not provide bounded goal-driven tool use.

2. Allow an LLM to call MediatR, repositories, EF Core, or arbitrary methods directly.

   Rejected because model output is untrusted and must not define executable backend behavior.

3. Add an API-layer bounded autonomous catalog sub-agent with a fixed tool registry, provider-neutral model contract, server-side validation, and trusted per-execution product identifiers.

   Accepted.

## Decision

The main assistant determines only whether a safe request belongs to the Catalog bounded context and delegates the original natural-language goal. The Catalog sub-agent owns a bounded model/tool loop with hard limits on iterations, tool calls, conversation messages, and search page size.

The only autonomous Catalog tools are:

- `catalog_search_products`, which always searches active public products through `SearchProductsQuery`.
- `catalog_get_product`, which retrieves an active product through `GetProductByIdQuery` only when its identifier was returned by a successful tool earlier in the same execution.

Tool names and arguments are validated by deterministic C# code. Tools are resolved from a catalog-specific registry and cannot construct arbitrary MediatR requests. Model-produced product fields are never authoritative. Final structured product DTOs are rebuilt only from trusted server-side tool results.

Maximum-price filtering is added to the existing Catalog query and applied in Infrastructure before count, ordering, and pagination. The existing strict "under" semantic (`Price < MaximumPrice`) is preserved. No schema change or migration is required.

Provider-specific HTTP mapping remains in the OpenAI-compatible and Gemini adapters. A provider-neutral `IAssistantLanguageModel` supplies structured tool or completion decisions to the catalog loop. Product names and descriptions in tool output are untrusted data and cannot alter tool policy.

When the autonomous catalog agent is disabled, catalog-agent execution fails closed with a stable unsupported response. Other assistant domains keep their existing behavior. This avoids maintaining a second catalog sequencing implementation that could drift from the autonomous tool policy.

## Rationale

This design provides real bounded autonomy without moving AI concerns into Catalog modules or weakening CQRS, module isolation, authorization, or trust boundaries. Server-side tool and identifier validation keeps the LLM in a proposal role, while Catalog Application and Infrastructure remain authoritative for data access and filtering.

## Consequences

Positive:

- Catalog tool sequencing can span multiple verified reads.
- Only active public products can enter trusted state.
- Detail identifiers cannot be invented by the user or model.
- Existing assistant response shapes remain usable by clients.
- Price filtering is complete and database-backed.

Tradeoffs:

- Provider adapters carry an additional structured-response contract.
- Model availability affects autonomous behavior when enabled.
- The API layer contains bounded orchestration and response composition code.
- Conversation continuity is request-scoped until signed or server-maintained state is separately approved.

## Risks

- Models may request malformed or inefficient tool sequences; hard limits and validation fail safely.
- Product descriptions may contain prompt-injection text; instructions and deterministic policy treat all tool data as untrusted.
- Provider JSON-schema behavior may differ; parsing is strict and provider failure is safe.
- Search remains substring-based and paginated, so the agent must not claim an exhaustive catalog result beyond returned metadata.
