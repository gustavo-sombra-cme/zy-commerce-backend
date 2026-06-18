# Frontend Contract

Last Updated: 2026-06-18

This file captures frontend-facing API contract details that are easy to miss from backend implementation alone.

## Catalog

### POST /api/catalog/products

Creates a Catalog product.

Authentication:

- Requires `Authorization: Bearer {accessToken}`.

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
