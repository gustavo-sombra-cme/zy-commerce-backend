# Prompt 084: Assistant Read-Only Views Execution

## Date

2026-06-24

## Purpose

Execute Task 1 for the future Text-to-SQL Assistant by adding an approved database view boundary and safe read-only database setup documentation.

## Full Prompt

Add the database view boundary for the future Text-to-SQL Assistant by creating an `assistant` schema and approved read-only assistant views using the normal application migration path where practical. Add safe documentation for manual read-only SQL user setup and local-only `ConnectionStrings:AssistantReadOnly` configuration. Do not create the real DB login/user, do not commit secrets or passwords, do not change assistant runtime behavior, do not change MCP, do not change frontend, and do not add SQL validator/executor or LLM Text-to-SQL planning yet. Run restore, build, test, and create a local commit only if verification passes.

## Scope

- Backend database migration artifact
- Project documentation
- Prompt execution log
- No runtime assistant behavior changes
- No MCP changes
- No frontend changes
- No real credentials

## Result Summary

Added an Orders-owned EF Core raw-SQL migration for the `assistant` schema and these views:

- `assistant.v_ProductSearch`
- `assistant.v_ProductDetails`
- `assistant.v_MyOrders`
- `assistant.v_MyOrderLines`
- `assistant.v_MyOrderSummary`

The views expose only safe Catalog product fields and owner-scoped Orders fields with `BuyerUserId`. They do not expose Auth tables, password hashes, tokens, secrets, or auth internals.

Added `docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md` with manual read-only SQL principal setup, `ConnectionStrings:AssistantReadOnly` user-secrets/environment-variable guidance, migration apply command, and manual verification checklist.

## Notes

The committed local development defaults use separate Catalog, Auth, and Orders databases. The assistant view migration requires a target database containing both `catalog.Products` and `orders.Orders` / `orders.OrderLines`, so local application requires developer-selected shared database setup or equivalent manual migration ordering.
