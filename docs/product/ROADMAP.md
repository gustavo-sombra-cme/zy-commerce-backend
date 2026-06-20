# Product Roadmap

Last updated: 2026-06-19

## Implemented

Backend:

- Clean Architecture modular monolith foundation.
- Catalog product lifecycle and price-on-create support.
- Auth register, login, JWT access token, bearer validation, current user.
- Orders create, list current user's summaries, get current user's order details.
- Product snapshot strategy for Orders.
- Health checks.
- Structured logging and correlation IDs.
- Swagger/OpenAPI for local Development.
- Protected MCP endpoint with allowlisted tools.
- Protected deterministic backend assistant endpoint for read-only Catalog/Orders questions.

Frontend:

- Angular v22 app foundation with standalone components and lazy routes.
- Auth login/register/current-user/session/guard flow.
- Protected shell navigation.
- Catalog/product details client and feature pages.
- Cart state and checkout request mapping.
- Orders list/details/checkout clients and pages.
- Correlation ID and authorization interceptors.
- MCP client/assistant skeleton.

## Partially Implemented

- Frontend MCP tool execution workflow.
- Frontend assistant UI.
- Catalog UI fields that anticipate richer product data than backend currently provides.

## Skeleton Only

- Mutating MCP confirmation workflow.

## Planned / Candidate

Near-term candidates:

- Frontend integration with backend assistant endpoint.
- External LLM provider integration behind the existing assistant orchestration boundary.
- Product price update command/endpoints.
- Orders Catalog validation/integration for product snapshots.
- Integration testing setup for backend/API/frontend workflows.
- Frontend/backend contract alignment for Catalog optional fields.

Platform candidates:

- API versioning.
- Configuration validation.
- Dedicated MCP authorization policies/scopes.
- MCP rate limiting.
- Frontend MCP client integration hardening.

Auth candidates:

- Refresh token strategy.
- Broader authorization policy strategy.

Commerce candidates:

- Inventory.
- Payments.
- Shipping.
- Discounts/coupons.
- Order cancellation/refunds.
- Advanced order status workflows.
- Customer profile integration.

Documentation candidates:

- Product docs maintenance checklist.
- README onboarding.
- Architecture diagrams.
- Fresh verification appendix after running backend and frontend test suites.

## Intentionally Absent Until Approved

- New modules.
- New projects.
- Microservices.
- Event bus.
- Distributed transactions.
- Startup auto-migrations.
- Protected Catalog reads.
- Raw SQL tools/endpoints.
- Browser-held AI provider secrets.

## Unknown / Not Verified

- No delivery dates are committed.
- No production release plan is documented in the inspected files.
