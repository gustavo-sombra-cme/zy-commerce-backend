# 095 - Assistant Broad Catalog Search Execution

Date: 2026-07-15

## Purpose

Add a read-only assistant capability for broad catalog product search by product name, SKU, or natural text using the existing Catalog Application search query.

## Full Prompt

```text
APPROVED: EXECUTE Add read-only assistant broad catalog search by product name, SKU, or text WITH LOCAL COMMIT

Repository:
C:\ZippyYum\Learning\zy-commerce-backend

Branch:
feature/backend-assistant-broad-catalog-search

Goal:
Add a read-only assistant capability that allows users to search catalog products by natural text, product name, SKU, or keyword using the existing Catalog Application search query.

Scope:
- Backend assistant/API layer only.
- Add assistant intent kind: CatalogSearchProducts.
- Route natural catalog search questions to CatalogSearchProducts.
- Use existing SearchProductsQuery.
- Search term should map to existing Catalog search support, currently SKU or Name.
- Use active-only public search: IsActive = true.
- Return maximum 10 assistant results.
- Handle the new intent inside CatalogAssistantSubAgent.
- Reuse existing CatalogProducts response contract.
- Add/update tests.
- Update project memory docs.
- Add prompt execution log.
- Commit locally only after all checks pass.
- Do not push automatically.

Do not change Text-to-SQL internals, OrdersAssistantSubAgent behavior, AssistantQueryResponse, frontend, MCP, migrations, database schema, admin/write assistant actions, raw SQL exposure, genericTable exposure, appsettings secrets, or real connection strings.
```

## Status

APPROVED, EXECUTED

## Result Summary

Added read-only assistant broad catalog search by product name, SKU, or text. The new `CatalogSearchProducts` intent routes natural product discovery questions to `CatalogAssistantSubAgent`, which calls the existing Catalog Application `SearchProductsQuery` with active-only public search and a maximum of 10 results.

The implementation reuses the existing `catalog_search` tool and `catalogProducts` structured response contract. Text-to-SQL internals, Orders assistant behavior, frontend contracts, MCP, database schema, migrations, `AssistantQueryResponse`, raw SQL exposure, `genericTable`, and admin/write assistant behavior were unchanged.
