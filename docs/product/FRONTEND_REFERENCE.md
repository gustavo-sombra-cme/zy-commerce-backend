# Frontend Reference

Last updated: 2026-06-18

Frontend repository: `c:\ZippyYum\Learning\zy-commerce-frontend`

## Implemented Routes

Route root: `../zy-commerce-frontend/src/app/app.routes.ts`.

- `/auth`: lazy loads `features/auth/auth.routes`.
- `/catalog`: protected shell route, lazy loads `features/catalog/catalog.routes`.
- `/cart`: protected shell route, lazy loads `features/cart/cart.routes`.
- `/products/:productId`: protected shell route, lazy loads `features/catalog/product-details.routes`.
- `/orders`: protected shell route, lazy loads `features/orders/orders.routes`.
- `/assistant`: protected shell route, lazy loads `features/mcp-assistant/mcp-assistant.routes`.
- `**`: redirects to `catalog`.

Feature routes:

- Auth: `../zy-commerce-frontend/src/app/features/auth/auth.routes.ts`
- Catalog: `../zy-commerce-frontend/src/app/features/catalog/catalog.routes.ts`
- Product details: `../zy-commerce-frontend/src/app/features/catalog/product-details.routes.ts`
- Cart: `../zy-commerce-frontend/src/app/features/cart/cart.routes.ts`
- Orders: `../zy-commerce-frontend/src/app/features/orders/orders.routes.ts`
- MCP assistant: `../zy-commerce-frontend/src/app/features/mcp-assistant/mcp-assistant.routes.ts`

## Implemented Components

- `AppComponent`: root router outlet.
- `AppShellComponent`: authenticated shell with navigation/current-user/logout behavior.
- `LoginPageComponent`, `RegisterPageComponent`: REST auth pages.
- `CatalogPageComponent`: product browsing/search UI.
- `ProductDetailsPageComponent`: product detail view and add-to-cart interaction.
- `CartPageComponent`: cart item display/update/remove/clear behavior.
- `CheckoutPageComponent`: creates backend order from cart items.
- `OrdersPageComponent`: paginated order summary page.
- `OrderDetailsPageComponent`: owned order details page.
- `McpAssistantPageComponent`: MCP assistant-facing page.
- `PageHeaderComponent`: shared UI primitive.

References: `../zy-commerce-frontend/src/app/features`, `../zy-commerce-frontend/src/app/core/layout`, `../zy-commerce-frontend/src/app/shared/ui/page-header`.

## Implemented Services and Clients

Core:

- `RuntimeConfigService`: runtime API/MCP/config access.
- `AuthApiClient`: login/register/current-user REST calls.
- `AuthSessionService`: token/session/user state.
- `authGuard`, `authChildGuard`: protected route access.
- `authorizationInterceptor`: attaches bearer token to allowlisted origins.
- `correlationIdInterceptor`: sends correlation IDs.
- `AppLoggerService`, `global-error.handler`: logging/error infrastructure.

Feature:

- `CatalogApiClient`: product list/details REST calls.
- `CartStateService`: session-storage cart state and display subtotal.
- `OrdersApiClient`: create/list/get order REST calls.
- `buildCreateOrderRequest`: maps cart items to backend order contract.

MCP:

- `McpToolRegistry`: frontend MCP tool metadata allowlist.
- `McpHttpClientService`: MCP HTTP client.
- `McpConfirmationService`: mutating tool confirmation skeleton.

## Implemented REST Integration

- Login: `POST /api/auth/users/login`.
- Register: `POST /api/auth/users/register`.
- Current user: `GET /api/auth/users/me`.
- Catalog list: `GET /api/catalog/products`.
- Product details: `GET /api/catalog/products/{productId}`.
- Create order: `POST /api/orders`.
- List orders: `GET /api/orders`.
- Get order: `GET /api/orders/{orderId}`.

References: `auth-api.client.ts`, `catalog-api.client.ts`, `orders-api.client.ts`.

## Partially Implemented

- MCP assistant route exists and MCP client code exists, but frontend project memory says real MCP execution is not implemented. References: `../zy-commerce-frontend/docs/project/CURRENT_STATE.md`, `../zy-commerce-frontend/src/app/features/mcp-assistant`, `../zy-commerce-frontend/src/app/mcp`.
- Frontend models include optional fields not fully represented by backend contracts, such as `currencyCode` and `imageUrl`. Reference: `../zy-commerce-frontend/src/app/features/catalog/data/catalog.models.ts`.

## Skeleton Only

- Mutating MCP confirmation workflow is metadata/skeleton only. Reference: `../zy-commerce-frontend/docs/project/REST_MCP_BOUNDARIES.md`.

## Intentionally Absent

- Frontend must not use MCP for login/register.
- Frontend must not call AI provider APIs directly with secrets.
- Runtime config must not contain secrets.

References: `../zy-commerce-frontend/docs/project/SECURITY.md`, `../zy-commerce-frontend/docs/project/REST_MCP_BOUNDARIES.md`.

## Unknown / Not Verified

- No fresh Angular build/test/audit commands were run for this documentation pack.
- Browser rendering and live backend integration were not manually verified.
