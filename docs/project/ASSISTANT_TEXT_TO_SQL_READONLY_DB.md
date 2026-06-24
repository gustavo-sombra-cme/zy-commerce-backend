# Assistant Text-to-SQL Read-Only Database Boundary

## Status

Task 1 adds only the database boundary for a future Text-to-SQL Assistant:

- `assistant` schema
- approved read-only assistant views
- manual read-only SQL principal setup guidance
- local-only `ConnectionStrings:AssistantReadOnly` guidance

Text-to-SQL runtime execution is not implemented yet. The existing assistant behavior is unchanged.

## Migration Strategy

The assistant views are added through the Orders EF Core migration path:

```powershell
dotnet ef database update --context OrdersDbContext --project src\Modules\Orders\Ecommerce.Orders.Infrastructure --startup-project src\Api\Ecommerce.Api
```

The migration is raw SQL because the views span the existing `catalog` and `orders` schemas and are not EF entity models.

Important: the target database must contain both the `catalog.Products` table and the `orders.Orders` / `orders.OrderLines` tables before applying this migration. The committed local development defaults use separate `ConnectionStrings:Catalog` and `ConnectionStrings:Orders` databases, so a developer may need to point those contexts at a shared local database, or otherwise apply equivalent existing Catalog and Orders migrations into the same assistant-readable database before applying this view migration.

## Approved Views

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

The developer must create the real login/user manually. Do not commit real passwords or real read-only connection strings.

Adapt this script locally for SQL Server Developer/Express, LocalDB, Windows integrated auth, or an existing SQL login:

```sql
-- Run in master if creating a SQL login.
USE [master];
GO

-- Replace this password locally before running. Do not commit the real password.
CREATE LOGIN [zy_assistant_reader]
WITH PASSWORD = '<replace-locally-with-strong-password>';
GO

-- Run in the application database that contains catalog, orders, and assistant schemas.
USE [<replace-with-application-database-name>];
GO

CREATE USER [zy_assistant_reader] FOR LOGIN [zy_assistant_reader];
GO

GRANT SELECT ON SCHEMA::[assistant] TO [zy_assistant_reader];
GO
```

Permission rules:

- Grant `SELECT` only on the `assistant` schema/views.
- Do not grant `SELECT` on `catalog`, `orders`, or `auth` base tables.
- Do not grant `INSERT`, `UPDATE`, `DELETE`, `EXECUTE`, `CREATE`, `ALTER`, `DROP`, ownership, or elevated database roles.
- Do not grant direct access to `auth.Users`.

## AssistantReadOnly Connection String

Future Task 2 will use:

```text
ConnectionStrings:AssistantReadOnly
```

Store it locally only, using user secrets or environment variables. Do not add a real password to `appsettings.json` or `appsettings.Development.json`.

User secrets example:

```powershell
dotnet user-secrets set "ConnectionStrings:AssistantReadOnly" "Server=...;Database=...;User Id=zy_assistant_reader;Password=...;TrustServerCertificate=True;" --project src\Api\Ecommerce.Api
```

Environment variable example:

```powershell
$env:ConnectionStrings__AssistantReadOnly="Server=...;Database=...;User Id=zy_assistant_reader;Password=...;TrustServerCertificate=True;"
```

## Manual Verification Checklist

After applying the migration and manually creating the read-only user:

1. Query all `assistant` views using a normal admin/app database connection.
2. Query all `assistant` views using `zy_assistant_reader`.
3. Attempt `SELECT` from `catalog.Products`, `orders.Orders`, `orders.OrderLines`, and `auth.Users` using `zy_assistant_reader`; confirm direct base-table access is denied.
4. Confirm the assistant views expose no password hashes, tokens, security stamps, auth internals, secrets, or connection strings.
5. Confirm order views include `BuyerUserId` for future authenticated-user scoping.
6. Store `ConnectionStrings:AssistantReadOnly` locally only.
