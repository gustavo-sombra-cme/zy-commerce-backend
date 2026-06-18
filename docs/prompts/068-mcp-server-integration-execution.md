# Prompt 068 - MCP Server Integration Execution

## Prompt Number

068

## Date

2026-06-12

## Purpose

Execute the approved MCP Server Integration platform improvement.

## Full Prompt

APPROVED: EXECUTE MCP Server Integration

Additional Explicit Approval:
- Approved to add the minimal official MCP ASP.NET Core package to the API project.

Additional Requirements:
- Implement MCP as an API-layer adapter under Ecommerce.Api only.
- Host protected /mcp using Streamable HTTP if supported by the installed SDK version.
- MCP handlers must call existing Application/CQRS requests through ISender.
- Do not call EF DbContexts, repositories, Domain objects, or module internals directly.
- Expose only the approved initial allowlist:
  - catalog_search_products
  - catalog_get_product_by_id
  - orders_get_order_by_id
  - orders_create_order only with explicit confirmedByUser input and tests
- Do not expose Auth register/login, JWTs, passwords, auth headers, raw database access, migrations, health readiness details, appsettings, environment variables, SQL, Catalog writes, cross-user orders, or non-existent Orders features.
- Add ADR-003 for MCP boundary and security.
- Keep Domain, Application, Infrastructure modules unchanged unless a compile-time adapter contract issue requires a minimal, justified change.

## Status

APPROVED

## Result Summary

Execution approved. Result summary will be updated after implementation and verification.
