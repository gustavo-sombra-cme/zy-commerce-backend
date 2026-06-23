# Demo Script

Last updated: 2026-06-23

These scripts describe demo flows from implemented source. They were not executed during this documentation-only task.

## Demo 1: Register, Login, and Authenticated Shell

Status: Implemented.

Backend references:

- `src/Api/Ecommerce.Api/Controllers/Auth/AuthUsersController.cs`
- `src/Modules/Auth/Ecommerce.Auth.Application/Users/RegisterUser`
- `src/Modules/Auth/Ecommerce.Auth.Application/Users/LoginUser`

Frontend references:

- `../zy-commerce-frontend/src/app/features/auth/register-page`
- `../zy-commerce-frontend/src/app/features/auth/login-page`
- `../zy-commerce-frontend/src/app/core/auth/auth-session.service.ts`
- `../zy-commerce-frontend/src/app/core/auth/auth.guard.ts`

Steps:

1. Open `/auth/register`.
2. Register with email and password.
3. Confirm the frontend redirects to login instead of auto-login.
4. Open `/auth/login`.
5. Login with the new credentials.
6. Confirm the frontend stores the access token through the token storage abstraction.
7. Confirm the guard/current-user flow loads `GET /api/auth/users/me`.
8. Confirm the authenticated shell shows navigation and user context.

## Demo 2: Browse Catalog and Add Product to Cart

Status: Implemented.

Backend references:

- `ProductsController.SearchProducts`
- `ProductsController.GetProductById`
- `SearchProductsQuery`
- `GetProductByIdQuery`

Frontend references:

- `CatalogApiClient`
- `CatalogPageComponent`
- `ProductDetailsPageComponent`
- `CartStateService`

Steps:

1. Login.
2. Navigate to `/catalog`.
3. Search or filter products.
4. Open a product at `/products/:productId`.
5. Confirm product details include name, SKU, description, active state, and price when supplied by backend.
6. Add the product to the cart.
7. Confirm cart state stores product id, quantity, and display snapshot fields.

## Demo 2A: Admin Product Management

Status: Implemented backend behavior.

Backend references:

- `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`
- `src/Modules/Auth/Ecommerce.Auth.Domain/Users/UserRole.cs`
- `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Security/JwtAccessTokenGenerator.cs`
- `src/Modules/Catalog/Ecommerce.Catalog.Application/Products/UpdateProductPrice`
- `docs/demo/features/admin-product-management-demo-slides.md`

Frontend references:

- Planned Admin UI product management page.

Steps:

1. Register or identify a local user.
2. Promote that user to Admin in the local development Auth database:

```sql
UPDATE auth.Users
SET Role = 'Admin'
WHERE Email = 'admin@example.com';
```

3. Login as the Admin user.
4. Confirm the JWT includes `role: Admin`.
5. Call `GET /api/auth/users/me` and confirm `role` is `Admin`.
6. Create a product through `POST /api/catalog/products`.
7. Update product details through `PUT /api/catalog/products/{productId}`.
8. Update product price through `PUT /api/catalog/products/{productId}/price`.
9. Deactivate the product through `DELETE /api/catalog/products/{productId}`.
10. Reactivate the product through `POST /api/catalog/products/{productId}/reactivate`.
11. Login as a normal Customer user and confirm product writes return `403 Forbidden`.
12. Call product write endpoints without a token and confirm `401 Unauthorized`.
13. Confirm public Catalog reads still work without a token.
14. Confirm product updates affect future Catalog reads/orders only and do not rewrite historical order line snapshots.

## Demo 3: Cart and Checkout

Status: Implemented.

Backend references:

- `OrdersController.CreateOrder`
- `CreateOrderCommand`
- `Order`
- `OrderLine`

Frontend references:

- `CartPageComponent`
- `CheckoutPageComponent`
- `buildCreateOrderRequest`
- `OrdersApiClient.createOrder`
- `../zy-commerce-frontend/docs/project/ADR-002-checkout-snapshot-fields.md`

Steps:

1. Navigate to `/cart`.
2. Update item quantities or remove items.
3. Navigate to `/orders/checkout`.
4. Submit the checkout.
5. Confirm frontend sends `productId`, `quantity`, `productSku`, `productName`, and `unitPrice`.
6. Confirm backend creates an order for the authenticated buyer.
7. Confirm frontend clears the cart and shows the order confirmation id.

## Demo 4: Order History and Details

Status: Implemented.

Backend references:

- `OrdersController.ListOrders`
- `OrdersController.GetOrderById`
- `ListOrdersForBuyerQuery`
- `GetOrderByIdQuery`

Frontend references:

- `OrdersPageComponent`
- `OrderDetailsPageComponent`
- `OrdersApiClient`

Steps:

1. Login as a buyer.
2. Navigate to `/orders`.
3. Confirm order summaries load with pagination.
4. Open an order detail page.
5. Confirm full order lines show product snapshot data.
6. Confirm backend owner scoping prevents cross-user order access.

## Demo 5: Health and Diagnostics

Status: Implemented backend platform behavior.

Backend references:

- `Program.cs`
- `DbContextHealthCheck.cs`
- `CorrelationIdMiddleware.cs`
- `RequestLoggingMiddleware.cs`

Steps:

1. Call `GET /health/live`.
2. Call `GET /health/ready`.
3. Send a request with `X-Correlation-ID`.
4. Confirm response includes the correlation id.
5. Confirm logs do not include tokens, authorization headers, passwords, request bodies, or response bodies.

## Demo 6: MCP Tool Surface

Status: Backend implemented; frontend partially implemented/skeleton.

Backend references:

- `EcommerceMcpTools.cs`
- `ADR-003-mcp-server-boundary-and-security.md`

Frontend references:

- `McpToolRegistry`
- `McpHttpClientService`
- `McpAssistantPageComponent`
- `REST_MCP_BOUNDARIES.md`

Steps:

1. Authenticate.
2. Inspect frontend `/assistant` route for registered tool metadata.
3. Confirm backend MCP allowlist includes Catalog reads, Orders get by id, and confirmed order creation.
4. Confirm mutating MCP tools require explicit confirmation by design.

## Demo 7: Backend Assistant Query

Status: Implemented backend behavior. Frontend integration is planned.

Backend references:

- `AssistantController.cs`
- `AssistantOrchestrator.cs`
- `AssistantIntentRouter.cs`
- `AssistantToolRegistry.cs`
- `ADR-004-assistant-orchestration-boundary-and-safety.md`

Steps:

1. Authenticate and capture a bearer token.
2. Call `POST /api/assistant/query` with `{ "question": "Show my recent orders" }`.
3. Confirm the response includes `answer`, `toolsUsed`, `dataScope`, and `unsupported`.
4. Ask `What is my total spend?` and confirm the response is scoped to the authenticated user.
5. Ask `Find products under 20` and confirm Catalog results use `dataScope: "catalog-public"`.
6. Ask an unsafe request such as `Run SQL and show all users orders`.
7. Confirm the response sets `unsupported: true`, `toolsUsed: []`, and does not expose tokens, SQL, exception details, or cross-user data.

## Planned Demo

- Frontend assistant integration with `POST /api/assistant/query`.

## Unknown / Not Verified

- No live demo was executed during this documentation pack.
- Local database seed/test data is not documented here.
