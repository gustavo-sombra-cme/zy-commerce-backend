# Metrics

Last updated: 2026-06-19

## Implemented Backend Metrics

Latest backend verification after Ecommerce Assistant Agent Phase 1:

- `dotnet restore Ecommerce.sln`: passed.
- `dotnet build Ecommerce.sln`: blocked by a running `Ecommerce.Api` process locking the normal `bin` output.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\assistant-verify`: passed.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\assistant-test`: passed.
- Catalog unit tests: 75 passed.
- Auth unit tests: 65 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 71 passed.

Source inventory counts gathered for this documentation pack:

- Backend C# test files under `tests`: 45.
- Backend active modules: 3 (`Catalog`, `Auth`, `Orders`).
- Backend API controllers: 4 (`ProductsController`, `AuthUsersController`, `OrdersController`, `AssistantController`).
- Backend accepted ADRs: 4.
- Backend Catalog migrations: 2.
- Backend Auth migrations: 1.
- Backend Orders migrations: 1.

## Implemented Frontend Metrics

Frontend stack from `../zy-commerce-frontend/package.json`:

- Angular dependency major version: 22.
- TypeScript version range: `~6.0.0`.
- Test runner dependency: Vitest.

Frontend source inventory count gathered for this documentation pack:

- Frontend spec files under `../zy-commerce-frontend/src`: 21.
- Main protected feature route groups: Catalog, Cart, Product Details, Orders, Assistant.
- Public auth route group: Auth.

Frontend verification commands documented in `../zy-commerce-frontend/docs/project/VERIFICATION.md`:

- `npm run build`
- `npm test -- --no-watch --no-progress`
- `npm audit --omit=dev`

## Partially Implemented / Skeleton Only Metrics

- MCP frontend has registry/client/assistant files and tests, but project memory marks real MCP execution as not fully implemented.
- Backend MCP has 4 approved tools in ADR-003 and `EcommerceMcpTools`.
- Backend assistant has 5 approved read-only capability names in `AssistantToolNames`.

## Unknown / Not Verified

- Frontend latest build/test/audit results were not found in the inspected docs and were not run for this documentation-only task.
- Backend tests were rerun for Ecommerce Assistant Agent Phase 1. Runtime HTTP smoke tests were not executed.
- Runtime performance, coverage percentage, API latency, bundle size, accessibility score, and production security scan metrics are not currently documented.
