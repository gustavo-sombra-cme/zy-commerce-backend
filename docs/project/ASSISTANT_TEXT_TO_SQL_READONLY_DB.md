# Assistant Text-to-SQL Read-Only Database Boundary

## Status

Task 1 added only the database boundary for a future Text-to-SQL Assistant:

- `assistant` schema
- approved read-only assistant views
- manual read-only SQL principal setup guidance
- local-only read-only connection string guidance

Task 2 adds a dormant SQL validator and read-only executor behind `Assistant:TextToSql:Enabled`. The existing assistant behavior is unchanged because Text-to-SQL planning and assistant orchestration wiring are not implemented yet.

## Migration Strategy

Catalog assistant views are added through the Catalog EF Core migration path:

```powershell
dotnet ef database update --context CatalogDbContext --project src\Modules\Catalog\Ecommerce.Catalog.Infrastructure --startup-project src\Api\Ecommerce.Api
```

Orders assistant views are added through the Orders EF Core migration path:

```powershell
dotnet ef database update --context OrdersDbContext --project src\Modules\Orders\Ecommerce.Orders.Infrastructure --startup-project src\Api\Ecommerce.Api
```

The migrations are raw SQL because these views are read-only database surfaces and are not EF entity models.

Important: Catalog and Orders use separate physical databases. Do not create cross-database views, linked servers, synonyms, or views that query Orders tables from the Catalog database or Catalog tables from the Orders database.

## Approved Views

Catalog database views:

`assistant.v_ProductSearch`

- `ProductId`
- `Name`
- `Sku`
- `Description`
- `PriceAmount`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`

`assistant.v_ProductDetails`

- `ProductId`
- `Name`
- `Sku`
- `Description`
- `PriceAmount`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`

Orders database views:

`assistant.v_MyOrders`

- `OrderId`
- `BuyerUserId`
- `Status`
- `TotalAmount`
- `CreatedAt`
- `LineCount`

`assistant.v_MyOrderLines`

- `OrderId`
- `BuyerUserId`
- `ProductId`
- `ProductName`
- `ProductSku`
- `Quantity`
- `UnitPriceAmount`
- `LineTotal`

`assistant.v_MyOrderSummary`

- `BuyerUserId`
- `TotalOrders`
- `TotalSpend`
- `LastOrderDate`

The current database schema does not contain order numbers or currency columns, so those columns are intentionally omitted. The views do not expose `auth.Users`, password hashes, tokens, security stamps, connection strings, or internal assistant/provider configuration.

## Manual Read-Only SQL Principal

The developer must create the real login/user manually in each database. Do not commit real passwords or real read-only connection strings.

Adapt this script locally for SQL Server Developer/Express, LocalDB, Windows integrated auth, or an existing SQL login:

```sql
-- Run in master if creating a SQL login.
USE [master];
GO

-- Replace this password locally before running. Do not commit the real password.
CREATE LOGIN [AssistantCatalogRo]
WITH PASSWORD = '<replace-locally-with-strong-password>';
GO

CREATE LOGIN [AssistantOrdersRo]
WITH PASSWORD = '<replace-locally-with-strong-password>';
GO

-- Run in the Catalog database after the Catalog assistant migration is applied.
USE [<replace-with-catalog-database-name>];
GO

CREATE USER [AssistantCatalogRo] FOR LOGIN [AssistantCatalogRo];
GO

GRANT SELECT ON SCHEMA::[assistant] TO [AssistantCatalogRo];
GO

-- Run in the Orders database after the Orders assistant migration is applied.
USE [<replace-with-orders-database-name>];
GO

CREATE USER [AssistantOrdersRo] FOR LOGIN [AssistantOrdersRo];
GO

GRANT SELECT ON SCHEMA::[assistant] TO [AssistantOrdersRo];
GO
```

Permission rules:

- Grant `SELECT` only on the `assistant` schema/views.
- Do not grant `SELECT` on `catalog`, `orders`, or `auth` base tables.
- Do not grant `INSERT`, `UPDATE`, `DELETE`, `EXECUTE`, `CREATE`, `ALTER`, `DROP`, ownership, or elevated database roles.
- Do not grant direct access to `auth.Users`.
- Do not use `db_datareader` or `db_owner`.

## Read-Only Connection Strings

Task 2 and later Text-to-SQL runtime code use:

```text
ConnectionStrings:AssistantCatalogReadOnly
ConnectionStrings:AssistantOrdersReadOnly
```

Store it locally only, using user secrets or environment variables. Do not add a real password to `appsettings.json` or `appsettings.Development.json`.

The normal application connection strings (`Catalog`, `Orders`, and `Auth`) must not be used for Text-to-SQL execution.

## Task 2 Validator And Executor

Task 2 adds only the backend SQL safety layer:

- `Assistant:TextToSql:Enabled` defaults to `false`.
- `Assistant:TextToSql:MaxRows` defaults to `50`.
- `Assistant:TextToSql:CommandTimeoutSeconds` defaults to `5`.
- SQL must validate before execution.
- Only `SELECT TOP (n)` queries over approved `assistant` views are accepted.
- Orders queries must include `BuyerUserId = @CurrentUserId`; the backend supplies `@CurrentUserId`.
- Raw database errors are not returned to callers.
- The LLM Text-to-SQL planner is not implemented until Task 3.
- Assistant orchestration is not wired to Text-to-SQL until Task 4.

User secrets example:

```powershell
dotnet user-secrets set "ConnectionStrings:AssistantCatalogReadOnly" "Server=...;Database=...;User Id=AssistantCatalogRo;Password=...;TrustServerCertificate=True;" --project src\Api\Ecommerce.Api
dotnet user-secrets set "ConnectionStrings:AssistantOrdersReadOnly" "Server=...;Database=...;User Id=AssistantOrdersRo;Password=...;TrustServerCertificate=True;" --project src\Api\Ecommerce.Api
```

Environment variable example:

```powershell
$env:ConnectionStrings__AssistantCatalogReadOnly="Server=...;Database=...;User Id=AssistantCatalogRo;Password=...;TrustServerCertificate=True;"
$env:ConnectionStrings__AssistantOrdersReadOnly="Server=...;Database=...;User Id=AssistantOrdersRo;Password=...;TrustServerCertificate=True;"
```

## Manual Verification Checklist

After applying both migrations and manually creating the read-only users:

1. Query Catalog `assistant` views using a normal Catalog admin/app database connection.
2. Query Orders `assistant` views using a normal Orders admin/app database connection.
3. Query Catalog `assistant` views using `AssistantCatalogRo`.
4. Query Orders `assistant` views using `AssistantOrdersRo`.
5. Attempt `SELECT` from `catalog.Products` using `AssistantCatalogRo`; confirm direct base-table access is denied.
6. Attempt `SELECT` from `orders.Orders` and `orders.OrderLines` using `AssistantOrdersRo`; confirm direct base-table access is denied.
7. Confirm neither read-only user has access to `auth.Users`.
8. Confirm the assistant views expose no password hashes, tokens, security stamps, auth internals, secrets, or connection strings.
9. Confirm order views include `BuyerUserId` for future authenticated-user scoping.
10. Store read-only connection strings locally only.
