# Prompt 084: Assistant Read-Only Views Execution

## Date

2026-06-24

## Purpose

Execute Task 1 for the future Text-to-SQL Assistant by adding an approved database view boundary and safe read-only database setup documentation.

## Full Prompt

Add the database view boundary for the future Text-to-SQL Assistant by creating an `assistant` schema and approved read-only assistant views using the normal application migration path where practical. Catalog and Orders use separate physical databases, so create Catalog assistant views in the Catalog migration path and Orders assistant views in the Orders migration path. Add safe documentation for manual read-only SQL user setup and local-only `ConnectionStrings:AssistantCatalogReadOnly` / `ConnectionStrings:AssistantOrdersReadOnly` configuration. Do not create the real DB login/user, do not commit secrets or passwords, do not change assistant runtime behavior, do not change MCP, do not change frontend, and do not add SQL validator/executor or LLM Text-to-SQL planning yet. Run restore, build, test, and create a local commit only if verification passes.

## Scope

- Backend database migration artifact
- Project documentation
- Prompt execution log
- No runtime assistant behavior changes
- No MCP changes
- No frontend changes
- No real credentials

## Result Summary

Added Catalog-owned and Orders-owned EF Core raw-SQL migrations for separate `assistant` schemas and these views:

Catalog database:

- `assistant.v_ProductSearch`
- `assistant.v_ProductDetails`

Orders database:

- `assistant.v_MyOrders`
- `assistant.v_MyOrderLines`
- `assistant.v_MyOrderSummary`

The views expose only safe Catalog product fields and owner-scoped Orders fields with `BuyerUserId`. They do not expose Auth tables, password hashes, tokens, secrets, or auth internals.

Added `docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md` with manual read-only SQL principal setup, `ConnectionStrings:AssistantCatalogReadOnly` and `ConnectionStrings:AssistantOrdersReadOnly` user-secrets/environment-variable guidance, migration apply commands, and manual verification checklist.

## Notes

The committed local development defaults use separate Catalog, Auth, and Orders databases. The assistant view migrations preserve that architecture and do not use cross-database views, linked servers, synonyms, or a combined Catalog/Orders database assumption.
