# 096 - Bounded Autonomous Catalog Agent Planning

Date: 2026-07-16

## Purpose

Plan the approved conversion of the deterministic Catalog assistant handler into a bounded autonomous catalog sub-agent.

## Full Prompt

```text
APPROVED: EXECUTE CONVERT CATALOG ASSISTANT INTO A BOUNDED AUTONOMOUS SUB-AGENT

Convert CatalogAssistantSubAgent from an intent switch into a goal-driven, bounded LLM loop. Preserve Clean Architecture, CQRS, MediatR, DDD, authorization, and existing assistant response contracts where possible. Add provider-neutral model/tool contracts; catalog_search_products and catalog_get_product tools; strict argument validation; active-only public reads; database-backed maximum-price filtering; a catalog-only tool registry; trusted per-execution product identifiers; structured final-response validation; bounded iterations, tool calls, messages, and page sizes; safe failures and structured logging; provider adapter support; top-level domain delegation; comprehensive fake-model tests; manual verification; an ADR; project-memory updates; and no secrets, writes, raw SQL, arbitrary tools, frontend changes, schema changes, migrations, pushes, or unrelated features.
```

## Status

PLANNED

## Result Summary

The design keeps domain routing in `AssistantOrchestrator`, moves catalog tool sequencing into an API-layer agent loop, keeps catalog reads behind existing MediatR queries, and adds no persistence schema. ADR-007 records the runtime autonomy and trust boundary.
