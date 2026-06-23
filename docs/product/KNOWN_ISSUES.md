# Known Issues and Gaps

Last updated: 2026-06-23

## Implemented Constraints

- Backend Catalog read endpoints are public by design. Reference: `docs/project/PROJECT_STATUS.md`.
- Backend Orders reads are owner-scoped to JWT `sub`; users cannot list or retrieve other users' orders through implemented endpoints. Reference: `OrdersController.cs`.
- Backend readiness checks require database connectivity but do not create databases or apply migrations. Reference: `DbContextHealthCheck.cs`.
- Backend assistant Phase 1 is deterministic and read-only. It supports a limited set of question families and returns safe unsupported responses for unclear, mutating, admin, SQL, token, database, internal, or cross-user requests. Reference: `docs/decisions/ADR-004-assistant-orchestration-boundary-and-safety.md`.

## Partially Implemented

- Frontend MCP exists as client infrastructure and assistant UI, but real execution is not fully implemented according to frontend project memory. References: `../zy-commerce-frontend/docs/project/CURRENT_STATE.md`, `../zy-commerce-frontend/src/app/mcp`.
- Frontend Catalog models include optional `currencyCode` and `imageUrl`, but backend Catalog contracts do not currently document currency/image as implemented fields. References: `catalog.models.ts`, `docs/project/FRONTEND_CONTRACT.md`.

## Skeleton Only

- Mutating MCP confirmation workflow is skeleton-only. Reference: `../zy-commerce-frontend/docs/project/REST_MCP_BOUNDARIES.md`.

## Planned / Future Work

- Orders product snapshot data can be spoofed until Catalog validation/integration is approved. Reference: `docs/decisions/ADR-002-orders-product-snapshot-strategy.md`.
- Auth refresh tokens, broader permissions beyond Customer/Admin, public admin registration, and token persistence are not implemented.
- Integration testing setup is not implemented.
- Frontend integration with backend `POST /api/assistant/query` is not implemented in this backend slice.
- External LLM-backed assistant orchestration is not implemented.
- Inventory, payments, shipping, discounts/coupons, cancellation/refunds, and advanced order workflows are not implemented.

## Intentionally Absent

- No raw SQL endpoints.
- No startup auto-migrations.
- No microservices, event bus, or distributed transactions.
- No frontend AI provider secrets or direct browser AI provider calls.
- No assistant mutating actions, raw SQL, admin analytics, or cross-user analysis.
- No MCP use for login/register.
- No MCP or Assistant admin tools.

## Unknown / Not Verified

- Local SQL Server LocalDB availability was not checked for this documentation pack.
- Current frontend build/test/audit status is unavailable from inspected docs.
- Live browser behavior was not verified.
- Production deployment configuration and runtime monitoring metrics are not documented.
