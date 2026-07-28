# Prompt 108: Assistant Product Comparison Planning

- **Prompt Number:** 108
- **Date:** 2026-07-28
- **Purpose:** Plan a read-only assistant capability that safely compares two active Catalog products resolved by natural name, SKU, or search text without guessing through ambiguous matches.
- **Status:** PLANNED

## Full Prompt

> Plan assistant product comparison by name or SKU.
>
> Goal:
> Add a read-only assistant capability that compares two active Catalog products resolved by natural name, SKU, or search text.
>
> Example questions:
> - compare Galaxy and iPhone
> - compare iPhone with headphones
> - compare SKU ABC123 with SKU MANUAL-IPHN-001
> - which is cheaper, Galaxy or iPhone?
> - what is the difference between laptop and headphones?
>
> Context:
> This feature is for tomorrow's demo. It should build on the existing assistant Catalog search and product-detail lookup behavior. The assistant must not guess when product matching is ambiguous.
>
> Expected behavior:
> - Resolve the first product using active-only Catalog search.
> - Resolve the second product using active-only Catalog search.
> - If both sides resolve to exactly one active product, load details and return a comparison.
> - If either side has zero matches, return a safe not-found/empty result.
> - If either side has multiple matches, return choices and do not guess.
> - Compare only safe public Catalog fields such as name, SKU, price, active status, and description.
> - Support "which is cheaper" using trusted product price values.
>
> Constraints:
> - Planning only.
> - Do not write code.
> - Do not modify files except required prompt log if repo rules require it.
> - Do not change frontend.
> - Do not change MCP.
> - Do not change Text-to-SQL internals.
> - Do not add migrations.
> - Do not add packages.
> - Do not add assistant write/admin behavior.
> - Do not expose raw SQL or genericTable.
> - Preserve Clean Architecture, CQRS, module isolation, and assistant safety rules.
> - Use selective loading. Do not read unrelated prompt logs or all historical project memory unless required.
>
> End with:
> PLAN_STATUS: PENDING_APPROVAL

## Result Summary

Planned an API-layer extension to the existing bounded autonomous Catalog agent for deterministic two-sided product comparison. The plan adds a validated comparison intent carrying two search terms and a comparison mode, reuses the active-only `catalog_search_products` and trusted-ID `catalog_get_product` tools, binds search evidence to each requested side, refuses to guess on zero or multiple matches, requires trusted detail results for both unique products, and builds comparison and cheaper-product wording from server-owned public fields and decimal prices.

The public assistant endpoint and existing `catalogProducts` structured response remain unchanged. No frontend, MCP, Text-to-SQL, Catalog module, persistence, migration, package, write/admin, raw SQL, or `genericTable` changes are planned. ADR-007 already permits verified product comparison within this bounded agent, so no ADR creation or update is required.
