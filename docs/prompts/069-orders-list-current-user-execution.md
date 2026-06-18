# Prompt 069 - Orders List Current User Execution

## Prompt Number

069

## Date

2026-06-17

## Purpose

Execute a query-side Orders feature that lists order summaries for the authenticated user with pagination.

## Full Prompt

APPROVED: EXECUTE Orders List For Current User

Additional Requirements:
- Add protected GET /api/orders.
- Return only orders owned by the authenticated JWT subject.
- Use query-side CQRS only.
- Do not add commands or domain behavior.
- Use pagination with defaults:
  - pageNumber = 1
  - pageSize = 20
  - max pageSize = 100
- Sort newest first by CreatedAt DESC.
- Return order summaries only:
  - orderId
  - status
  - totalAmount
  - createdAt
  - lineCount
- Do not expose full order lines in the list response.
- Do not add MCP changes.
- Do not add migrations unless execution discovers an unavoidable schema issue and stops for approval.
- Update frontend contract documentation.

## Status

EXECUTED

## Result Summary

Implemented protected `GET /api/orders` using query-side CQRS only. The endpoint scopes results to the authenticated JWT `sub` buyer id, applies pagination defaults `pageNumber=1` and `pageSize=20`, rejects `pageSize > 100`, sorts newest first by `CreatedAt` descending, and returns summary fields only. Added focused Orders unit tests, architecture authorization and summary-only contract coverage, and frontend contract documentation. No commands, domain behavior, MCP changes, migrations, schema changes, or packages were added. `dotnet restore Ecommerce.sln`, `dotnet build Ecommerce.sln --no-restore`, and `dotnet test Ecommerce.sln --no-build` passed.
