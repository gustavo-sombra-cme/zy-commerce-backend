# Assistant Order Product Match Mapping Bugfix Execution

## Scope

Backend-only Text-to-SQL assistant bugfix for read-only order-history questions that search order lines by product name or SKU.

## Changes

- Added Text-to-SQL planner prompt examples for first/earliest orders containing a product name and for orders where the authenticated user bought a product.
- Kept product-name order-history searches on the Orders data source with `resultShape=orderList`.
- Extended `orderList` response mapping so rows containing both order-card columns and order-line columns hydrate existing `AssistantOrderCardDto.Lines`.
- Preserved the public response contract by continuing to return `responseType=recentOrders` with `AssistantOrdersData`.
- Preserved fallback behavior for planner, validation, execution, generic table, and unmappable failures.
- Kept write/admin requests unsupported.

## Safety Notes

- No frontend changes.
- No MCP changes.
- No database schema changes or migrations.
- No `AssistantQueryResponse` contract changes.
- No public `genericTable` exposure.
- Generated SQL remains outside API responses.
- Orders SQL validation still requires `BuyerUserId = @CurrentUserId`.

## Verification

Run:

```powershell
dotnet restore Ecommerce.sln
dotnet build Ecommerce.sln
dotnet test Ecommerce.sln
```

Manual checks with Text-to-SQL enabled locally:

- `I need my first order where I order a Galaxy product`
- `show my orders where I bought Galaxy`
- `what is my last order`
- `deactivate product`

Expected:

- Galaxy order questions return `unsupported=false` when matching data exists.
- Matching Galaxy results use `responseType=recentOrders`.
- Matching results include order data and matching line details when present.
- Generated SQL is not returned in response data, tools, metadata, or answer text.
- `deactivate product` remains `unsupported=true`.
