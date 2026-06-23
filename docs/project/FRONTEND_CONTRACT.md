# Frontend Contract

Last Updated: 2026-06-23

This file captures frontend-facing API contract details that are easy to miss from backend implementation alone.

## Auth

### GET /api/auth/users/me

Gets the current authenticated user.

Authentication:

- Requires `Authorization: Bearer {accessToken}`.

Response:

```json
{
  "userId": "00000000-0000-0000-0000-000000000000",
  "email": "admin@example.com",
  "role": "Admin"
}
```

Role values currently used by the backend:

- `Customer`
- `Admin`

Frontend Admin UI should use this role for navigation/visibility, but the backend remains the source of truth. Admin-only product writes return `403 Forbidden` for authenticated non-admin users.

## Catalog

### POST /api/catalog/products

Creates a Catalog product.

Authentication:

- Requires `Authorization: Bearer {accessToken}` for an Admin user.
- Missing/invalid token returns `401 Unauthorized`.
- Authenticated non-admin users receive `403 Forbidden`.

Request:

```json
{
  "sku": "SKU-001",
  "name": "Test Product",
  "description": "Optional description",
  "price": 19.99
}
```

Request rules:

- `price` is required.
- `price` must be greater than or equal to `0`.
- The backend stores price with two decimal places.
- Product creation affects future Catalog reads and future Orders only. It does not change historical order line snapshots.

### PUT /api/catalog/products/{productId}

Updates Catalog product details.

Authentication:

- Requires `Authorization: Bearer {accessToken}` for an Admin user.
- Missing/invalid token returns `401 Unauthorized`.
- Authenticated non-admin users receive `403 Forbidden`.

Request:

```json
{
  "name": "Updated Product",
  "description": "Updated optional description"
}
```

Rules:

- Updates name and description only.
- Does not update SKU or price.
- Does not rewrite historical order line snapshots.

### PUT /api/catalog/products/{productId}/price

Updates Catalog product price.

Authentication:

- Requires `Authorization: Bearer {accessToken}` for an Admin user.
- Missing/invalid token returns `401 Unauthorized`.
- Authenticated non-admin users receive `403 Forbidden`.

Request:

```json
{
  "price": 29.99
}
```

Rules:

- `price` is required.
- `price` must be greater than or equal to `0`.
- Successful update returns `204 No Content`.
- Invalid price returns `400 Bad Request`.
- Missing product returns `404 Not Found`.
- Updated price affects future Catalog reads and future order snapshots only. Existing Orders keep their captured order line prices.

### DELETE /api/catalog/products/{productId}

Deactivates a Catalog product.

Authentication:

- Requires `Authorization: Bearer {accessToken}` for an Admin user.
- Missing/invalid token returns `401 Unauthorized`.
- Authenticated non-admin users receive `403 Forbidden`.

### POST /api/catalog/products/{productId}/reactivate

Reactivates a Catalog product.

Authentication:

- Requires `Authorization: Bearer {accessToken}` for an Admin user.
- Missing/invalid token returns `401 Unauthorized`.
- Authenticated non-admin users receive `403 Forbidden`.

### GET /api/catalog/products

Lists Catalog products.

- Public endpoint.
- Each item includes `price`.
- Frontend cart and checkout screens should use this Catalog price for display snapshots before creating order snapshot lines.

Item shape:

```json
{
  "productId": "00000000-0000-0000-0000-000000000000",
  "sku": "SKU-001",
  "name": "Test Product",
  "description": "Optional description",
  "price": 19.99,
  "isActive": true,
  "createdAt": "2026-06-18T12:00:00+00:00"
}
```

### GET /api/catalog/products/{productId}

Gets one Catalog product.

- Public endpoint.
- Response includes `price`.

Response `200 OK`:

```json
{
  "productId": "00000000-0000-0000-0000-000000000000",
  "sku": "SKU-001",
  "name": "Test Product",
  "description": "Optional description",
  "price": 19.99,
  "isActive": true,
  "createdAt": "2026-06-18T12:00:00+00:00",
  "updatedAt": null
}
```

## Orders

### GET /api/orders

Lists order summaries for the authenticated user.

Authentication:

- Requires `Authorization: Bearer {accessToken}`.
- The backend uses the JWT `sub` claim as the buyer id.
- Only orders owned by that buyer id are returned.

Query parameters:

- `pageNumber`: optional integer, defaults to `1`, must be `>= 1`.
- `pageSize`: optional integer, defaults to `20`, must be between `1` and `100`.

Sorting:

- Newest first by `createdAt` descending.

Response `200 OK`:

```json
{
  "items": [
    {
      "orderId": "00000000-0000-0000-0000-000000000000",
      "status": "Created",
      "totalAmount": 42.50,
      "createdAt": "2026-06-17T12:00:00+00:00",
      "lineCount": 2
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

List response rules:

- `items` contains summaries only.
- Order lines are not included in this response.
- Use `GET /api/orders/{orderId}` for full order details with lines.

Error responses:

- `400 Bad Request` for invalid pagination values.
- `401 Unauthorized` when the bearer token is missing, invalid, or does not contain a valid `sub` claim.

### GET /api/orders/{orderId}

Gets one order owned by the authenticated user.

- Requires `Authorization: Bearer {accessToken}`.
- Returns `404 Not Found` when the order does not exist or belongs to another user.
- Includes full order lines.

## Assistant

### POST /api/assistant/query

Answers supported ecommerce questions by composing backend-approved read-only Catalog and Orders capabilities.

Authentication:

- Requires `Authorization: Bearer {accessToken}`.
- The backend uses the JWT `sub` claim for all Orders analysis.
- Do not send `userId` or `buyerId`; the request body does not support owner-scope inputs.

Request:

```json
{
  "question": "What products did I order?"
}
```

Response `200 OK`:

```json
{
  "answer": "You ordered: Tea (3 total).",
  "toolsUsed": ["orders_search", "orders_get_order", "orders_analyze"],
  "dataScope": "authenticated-user",
  "unsupported": false,
  "responseType": "orderedProducts",
  "data": {
    "products": [
      {
        "productSku": "SKU-1",
        "productName": "Tea",
        "quantity": 3
      }
    ]
  }
}
```

`responseType` and `data` are optional nullable fields. Existing clients can ignore them and continue rendering `answer`; `answer` remains the authoritative plain-text fallback for every response.

Structured response types currently include `recentOrders`, `orderSummaryAnalytics`, `orderedProducts`, `matchingOrders`, `productFrequency`, `catalogProducts`, and `catalogProduct`.

Safe unsupported response shape:

```json
{
  "answer": "I can help with read-only product lookup and your own order history, but I cannot perform that request.",
  "toolsUsed": [],
  "dataScope": "none",
  "unsupported": true,
  "responseType": null,
  "data": null
}
```

Phase 1 supported question families:

- Recent orders.
- Products ordered.
- Orders containing a product id, SKU, or name.
- Total spend.
- Most frequently purchased products.
- Products under an amount.
- Orders containing products over an amount.

Rules:

- Provider-backed intent interpretation is optional configuration; deterministic fallback remains available.
- Read-only only.
- Mutating, admin, SQL, token, database, internal, unclear, and cross-user questions return `unsupported: true`.

## Local Admin Setup For Development

The backend does not expose a public admin registration endpoint and does not commit admin credentials.

For local development only:

1. Register a user through `POST /api/auth/users/register`.
2. Promote that user directly in the local Auth database.

Example SQL for LocalDB:

```sql
UPDATE auth.Users
SET Role = 'Admin'
WHERE Email = 'admin@example.com';
```

Then login normally through `POST /api/auth/users/login`. The returned JWT includes a `role` claim, and `GET /api/auth/users/me` returns `role`.
