# API Reference

Last updated: 2026-06-23

Base URL is environment-specific. Frontend reads it from runtime config: `../zy-commerce-frontend/src/app/core/config/runtime-config.service.ts`.

## Implemented Auth API

### POST /api/auth/users/register

Status: Implemented.

Reference: `src/Api/Ecommerce.Api/Controllers/Auth/AuthUsersController.cs`.

Request contract: `src/Modules/Auth/Ecommerce.Auth.Contracts/Users/RegisterUserRequest.cs`.

Response contract: `RegisterUserResponse.cs`.

Notes:

- Public endpoint.
- Duplicate email maps to conflict through global exception handling.

### POST /api/auth/users/login

Status: Implemented.

Reference: `AuthUsersController.cs`.

Request contract: `LoginUserRequest.cs`.

Response contract: `LoginUserResponse.cs`.

Notes:

- Public endpoint.
- Returns access token, token type, expiration, and user identity fields.
- Frontend stores access token through `AuthSessionService` and token storage abstraction.

### GET /api/auth/users/me

Status: Implemented.

Reference: `AuthUsersController.cs`.

Response contract: `GetCurrentUserResponse.cs`.

Notes:

- Protected by bearer authentication.
- Response includes `userId`, `email`, and `role`.
- Role values currently used by the backend are `Customer` and `Admin`.
- Frontend route guard calls this endpoint when a token exists but user state is not loaded.

## Implemented Catalog API

### GET /api/catalog/products

Status: Implemented.

Reference: `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`.

Query parameters:

- `searchTerm`: optional.
- `isActive`: optional boolean.
- `pageNumber`: optional integer.
- `pageSize`: optional integer.

Response contract: `src/Modules/Catalog/Ecommerce.Catalog.Contracts/Products/SearchProductsResponse.cs`.

Response items include product id, SKU, name, description, price, active state, and created timestamp.

### GET /api/catalog/products/{productId}

Status: Implemented.

Reference: `ProductsController.cs`.

Response contract: `GetProductByIdResponse.cs`.

Notes:

- Public endpoint.
- `Guid.Empty` is rejected by controller transport validation.
- Returns `404` when missing.

### POST /api/catalog/products

Status: Implemented.

Reference: `ProductsController.cs`.

Request contract: `CreateProductRequest.cs`.

Request fields:

- `sku`
- `name`
- `description`
- `price`

Notes:

- Admin-protected endpoint. Requires bearer token with role `Admin`.
- Missing/invalid token returns `401`.
- Authenticated non-admin user returns `403`.
- Price must be `>= 0`.
- Duplicate SKU maps to conflict.

### PUT /api/catalog/products/{productId}

Status: Implemented.

Reference: `ProductsController.cs`.

Request contract: `UpdateProductDetailsRequest.cs`.

Notes:

- Admin-protected endpoint. Requires bearer token with role `Admin`.
- Missing/invalid token returns `401`.
- Authenticated non-admin user returns `403`.
- Updates name and description only.
- Does not update SKU or price.
- Does not rewrite historical order line snapshots.

### PUT /api/catalog/products/{productId}/price

Status: Implemented.

Reference: `ProductsController.cs`.

Request contract: `UpdateProductPriceRequest.cs`.

Request fields:

- `price`

Notes:

- Admin-protected endpoint. Requires bearer token with role `Admin`.
- Missing/invalid token returns `401`.
- Authenticated non-admin user returns `403`.
- Updates price only.
- Price must be `>= 0`.
- Successful update returns `204 No Content`.
- Missing product maps to not found through global exception handling.
- Price updates affect future Catalog reads and future order snapshots only; existing Orders keep historical line prices.

### DELETE /api/catalog/products/{productId}

Status: Implemented.

Reference: `ProductsController.cs`.

Notes:

- Admin-protected endpoint. Requires bearer token with role `Admin`.
- Missing/invalid token returns `401`.
- Authenticated non-admin user returns `403`.
- Deactivates product.

### POST /api/catalog/products/{productId}/reactivate

Status: Implemented.

Reference: `ProductsController.cs`.

Notes:

- Admin-protected endpoint. Requires bearer token with role `Admin`.
- Missing/invalid token returns `401`.
- Authenticated non-admin user returns `403`.
- Reactivation is idempotent when the product is already active.

## Implemented Orders API

### POST /api/orders

Status: Implemented.

Reference: `src/Api/Ecommerce.Api/Controllers/Orders/OrdersController.cs`.

Request contract: `src/Modules/Orders/Ecommerce.Orders.Contracts/Orders/CreateOrderRequest.cs`.

Line fields:

- `productId`
- `productSku`
- `productName`
- `unitPrice`
- `quantity`

Notes:

- Protected endpoint.
- Buyer id comes from JWT `sub`.
- Frontend builds snapshot fields in `../zy-commerce-frontend/src/app/features/orders/data/create-order.mapper.ts`.

### GET /api/orders

Status: Implemented.

Reference: `OrdersController.cs`.

Query parameters:

- `pageNumber`: optional, defaults to `1`.
- `pageSize`: optional, defaults to `20`, max `100`.

Response contract: `ListOrdersResponse.cs`.

Notes:

- Protected endpoint.
- Owner-scoped to JWT `sub`.
- Returns summaries only; order lines are not included.

### GET /api/orders/{orderId}

Status: Implemented.

Reference: `OrdersController.cs`.

Response contract: `GetOrderByIdResponse.cs`.

Notes:

- Protected endpoint.
- Owner-scoped to JWT `sub`.
- Returns `404` for missing or cross-user orders.

## Implemented Platform API

### POST /api/assistant/query

Status: Implemented.

Reference: `src/Api/Ecommerce.Api/Controllers/Assistant/AssistantController.cs`.

Request contract: `src/Api/Ecommerce.Api/Assistant/AssistantQueryRequest.cs`.

Response contract: `src/Api/Ecommerce.Api/Assistant/AssistantQueryResponse.cs`.

Notes:

- Protected endpoint.
- Accepts a natural-language `question`.
- Preserves the plain-text `answer` for all responses.
- Adds optional nullable `responseType` and `data` fields when structured order, product, or analytics data is available.
- Existing clients can ignore `responseType` and `data`; the fields are additive and do not replace `answer`.
- Supported structured response types: `recentOrders`, `orderSummaryAnalytics`, `orderedProducts`, `matchingOrders`, `productFrequency`, `catalogProducts`, `catalogProduct`.
- Internal read-only capability allowlist: `catalog_search`, `catalog_get_product`, `orders_search`, `orders_get_order`, `orders_analyze`.
- Orders analysis is scoped to the authenticated JWT `sub` claim.
- Does not accept `userId` or `buyerId` from the request body.
- Mutating, admin, SQL, token, database, internal, unclear, and cross-user requests return a safe unsupported response with `unsupported=true`, `dataScope="none"`, empty `toolsUsed`, `responseType=null`, and `data=null`.
- The assistant can use deterministic interpretation, the existing OpenAI-style provider, or Gemini as a POC/testing provider through backend configuration only; the HTTP request/response contract does not change.
- Gemini configuration uses `ECOMMERCE_ASSISTANT_LLM_PROVIDER=Gemini`, `ECOMMERCE_ASSISTANT_GEMINI_API_KEY`, `ECOMMERCE_ASSISTANT_GEMINI_MODEL`, and `ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT`. API keys must come from environment variables, user secrets, or another non-committed configuration provider.
- Gemini free-tier and rate-limit behavior depends on the Google account/project. A ChatGPT/OpenAI subscription is unrelated to Gemini Developer API access.
- Provider failures, rate limits, timeouts, malformed provider JSON, and invalid intent-plan JSON fall back safely to deterministic behavior where possible.
- The assistant does not log prompts, raw provider responses, API keys, JWTs, auth headers, or sensitive payloads.

### GET /health/live

Status: Implemented.

Reference: `src/Api/Ecommerce.Api/Program.cs`.

Notes:

- Process liveness only.
- Does not depend on database connectivity.

### GET /health/ready

Status: Implemented.

Reference: `Program.cs`, `src/Api/Ecommerce.Api/HealthChecks/DbContextHealthCheck.cs`.

Notes:

- Checks Auth, Catalog, and Orders database connectivity.
- Does not create databases, apply migrations, or change schema.

### POST /mcp

Status: Implemented.

Reference: `src/Api/Ecommerce.Api/Mcp/EcommerceMcpTools.cs`.

Notes:

- Protected endpoint.
- Uses stateless Streamable HTTP MCP.
- Approved tools: `catalog_search_products`, `catalog_get_product_by_id`, `orders_get_order_by_id`, `orders_create_order`.
- `orders_create_order` requires explicit `confirmedByUser`.

## Partially Implemented / Skeleton Only

- Frontend MCP client has models, registry, HTTP client, and assistant page, but frontend docs mark real MCP execution as not fully implemented. References: `../zy-commerce-frontend/src/app/mcp`, `../zy-commerce-frontend/docs/project/CURRENT_STATE.md`.

## Planned

- Admin UI frontend integration.

## Intentionally Absent

- Raw SQL endpoints.
- Admin analytics endpoints.
- Customer/profile endpoints.
- Payment/shipping APIs.
- Inventory APIs.
- Public admin registration endpoint.
- MCP admin tools.
- Assistant admin tools.

## Unknown / Not Verified

- Swagger JSON was not regenerated for this documentation pack.
- Runtime HTTP calls were not executed.
