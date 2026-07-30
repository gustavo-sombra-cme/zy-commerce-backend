# Prompt 111: Assistant Product Availability And Count Planning

- **Prompt Number:** 111
- **Date:** 2026-07-30
- **Purpose:** Plan a read-only assistant capability that reports whether active Catalog products exist and how many match a natural name, SKU, or search text.
- **Status:** PLANNED

## Full Prompt

> Plan assistant product availability and count by search text.
>
> Goal:
> Add a small read-only assistant capability that answers whether active Catalog products exist and how many active products match a natural name, SKU, or search text.
>
> Example questions:
> - do you have any Galaxy products?
> - how many iPhone products do you have?
> - do you have any products matching headphones?
> - are there any products for gaming?
>
> Expected behavior:
> - Use existing Catalog Application/CQRS read paths.
> - Search active products only.
> - Use trusted server-side search results and TotalCount.
> - Return a clear availability/count answer.
> - Include matching product cards when useful.
> - Preserve existing broad search, description search, product detail lookup, and product comparison behavior.
>
> Constraints:
> - Planning only.
> - Do not write code.
> - Do not change frontend.
> - Do not change MCP.
> - Do not change Text-to-SQL internals.
> - Do not add migrations.
> - Do not add packages.
> - Do not add assistant write/admin behavior.
> - Do not expose raw SQL or genericTable.
> - Use selective loading.
>
> End with:
> PLAN_STATUS: PENDING_APPROVAL

## Result Summary

Planned a narrow API-layer extension to the bounded autonomous Catalog agent for availability and count questions. A conservative request parser will recognize explicit availability/count forms before broad Catalog search routing, extract one validated search term, and require one exact active-only `catalog_search_products` result. The deterministic finalizer will use trusted server-side `TotalCount` for the answer and rebuild an optional bounded set of product cards only from the same trusted result, including correct handling when the total exceeds the displayed page.

The plan reuses the existing Catalog Application `SearchProductsQuery`, Catalog tool, `catalogProducts` response type, and public product card fields. It preserves broad search, description search, detail lookup, and comparison routing; adds no frontend, MCP, Text-to-SQL, persistence, migration, package, write/admin, raw SQL, or `genericTable` behavior. ADR-004 and ADR-007 already govern this API-layer read orchestration and trusted Catalog tool flow, so no ADR creation or update is required.
