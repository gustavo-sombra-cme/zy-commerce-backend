# Feature Catalog

Last updated: 2026-06-19

## Implemented

### Backend Catalog

- Create product: protected `POST /api/catalog/products`; command/handler/validator in `CreateProduct`.
- Product price on create: `CreateProductRequest.Price`, `CreateProductCommand.Price`, `Product.Price`, migration `AddProductPrice`.
- Product details: public `GET /api/catalog/products/{productId}`.
- Product search/list: public `GET /api/catalog/products` with `searchTerm`, `isActive`, `pageNumber`, `pageSize`.
- Update details: protected `PUT /api/catalog/products/{productId}` for name/description only.
- Deactivate: protected `DELETE /api/catalog/products/{productId}`.
- Reactivate: protected `POST /api/catalog/products/{productId}/reactivate`.

References: `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`, `src/Modules/Catalog/Ecommerce.Catalog.Application/Products`, `tests/UnitTests/Ecommerce.Catalog.UnitTests/Products`.

### Backend Auth

- Register user: `POST /api/auth/users/register`.
- Login user: `POST /api/auth/users/login`.
- Current user: protected `GET /api/auth/users/me`.
- JWT access token generation and bearer validation.

References: `src/Api/Ecommerce.Api/Controllers/Auth/AuthUsersController.cs`, `src/Modules/Auth/Ecommerce.Auth.Application/Users`, `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Security`.

### Backend Orders

- Create order: protected `POST /api/orders`.
- List current user's order summaries: protected `GET /api/orders`.
- Get current user's order by id: protected `GET /api/orders/{orderId}`.
- Product snapshot fields on order lines.
- Owner scoping through JWT `sub`.

References: `src/Api/Ecommerce.Api/Controllers/Orders/OrdersController.cs`, `src/Modules/Orders/Ecommerce.Orders.Application/Orders`, `src/Modules/Orders/Ecommerce.Orders.Domain/Orders`.

### Backend Platform

- Health liveness/readiness: `/health/live`, `/health/ready`.
- Correlation ID middleware.
- Structured request/error/auth/health logging.
- Swagger/OpenAPI in Development.
- Protected MCP endpoint `/mcp`.
- Protected deterministic assistant endpoint `POST /api/assistant/query` with read-only Catalog/Orders capability orchestration.

References: `src/Api/Ecommerce.Api/Program.cs`, `src/Api/Ecommerce.Api/HealthChecks`, `src/Api/Ecommerce.Api/Middleware`, `src/Api/Ecommerce.Api/Mcp`, `src/Api/Ecommerce.Api/Assistant`, `src/Api/Ecommerce.Api/Controllers/Assistant`.

### Frontend Auth and Shell

- Login page: `/auth/login`.
- Register page: `/auth/register`.
- Authenticated shell with protected child routes.
- Guard loads current user when a token exists.
- Logout clears token and current user state.

References: `../zy-commerce-frontend/src/app/features/auth`, `../zy-commerce-frontend/src/app/core/auth`, `../zy-commerce-frontend/src/app/core/layout/app-shell`.

### Frontend Catalog, Cart, and Orders

- Catalog route: `/catalog`.
- Product details route: `/products/:productId`.
- Cart route: `/cart`.
- Orders route: `/orders`.
- Checkout route: `/orders/checkout`.
- Order details route: `/orders/:orderId`.
- Catalog and Orders REST API clients.
- Cart state with session storage persistence.
- Checkout request mapper for backend-required snapshot fields.

References: `../zy-commerce-frontend/src/app/features/catalog`, `../zy-commerce-frontend/src/app/features/cart`, `../zy-commerce-frontend/src/app/features/orders`.

## Partially Implemented

- Frontend MCP assistant route `/assistant` and MCP HTTP client/registry are present. The frontend project docs still classify MCP execution as not fully implemented. References: `../zy-commerce-frontend/src/app/features/mcp-assistant`, `../zy-commerce-frontend/src/app/mcp`, `../zy-commerce-frontend/docs/project/CURRENT_STATE.md`.

## Skeleton Only

- Mutating MCP confirmation flow: `McpConfirmationService` and tool metadata exist; final confirmation UI/workflow is still future-facing. Reference: `../zy-commerce-frontend/docs/project/REST_MCP_BOUNDARIES.md`.

## Planned

- Frontend integration with backend assistant endpoint.
- External LLM provider integration behind the existing backend assistant safety boundary.
- Catalog price update endpoint/command.
- Orders Catalog validation/integration.
- Auth refresh token strategy.
- Additional MCP policies/rate limiting.
- Integration testing setup.

References: `docs/project/ROADMAP.md`, `docs/decisions/ADR-004-assistant-orchestration-boundary-and-safety.md`.

## Intentionally Absent

- Inventory, Customers, Payments, Shipping, Promotions, Reviews, Notifications, Audit.
- Payments, inventory reservation, discounts/coupons, refunds, cancellation, advanced order status.
- Auth roles/permissions, refresh tokens, token persistence.
- Protected Catalog reads.

## Unknown / Not Verified

- Browser runtime behavior was not re-tested for this documentation pack.
- Local database availability was not verified.
