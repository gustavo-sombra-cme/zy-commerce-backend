# ADR-001: Product Search Read Model

## Date

2026-06-08

## Status

Accepted

## Context

Catalog uses a DDD Product aggregate with value objects for core fields:

- `Product.Sku` uses the `Sku` value object.
- `Product.Name` uses the `ProductName` value object.

EF Core maps these value objects to scalar SQL columns through value converters. Product search needs server-side filtering by SKU or name while keeping the domain model intact.

## Problem

EF Core could not translate search expressions over value object members such as `Product.Sku.Value` and `Product.Name.Value`.

Using `EF.Property<string>` directly against the converted aggregate properties also failed because EF still treated those mapped properties as value-object properties during conversion.

## Options Considered

1. Query aggregate value object members directly.

   Rejected because EF Core cannot reliably translate value object member access to SQL.

2. Use `AsEnumerable` and filter in memory.

   Rejected because filtering must remain server-side.

3. Remove value objects from the Product aggregate.

   Rejected because it weakens the domain model.

4. Use raw SQL.

   Rejected for now because it is unnecessary for this query and raw SQL requires explicit approval.

5. Add scalar query properties to the domain aggregate.

   Rejected because it pollutes the domain model with query/persistence concerns.

6. Introduce an infrastructure-only read model and read DbContext.

   Accepted.

## Decision

Introduce `ProductSearchReadModel` and `CatalogReadDbContext` in Catalog Infrastructure.

The Product aggregate remains the write model. Product search uses the read model, which maps scalar `Sku` and `Name` properties to the existing `catalog.Products` table.

The read model is keyless, infrastructure-only, and excluded from migrations.

## Rationale

This keeps DDD value objects in the Product aggregate while allowing EF Core to translate product search to SQL.

It also keeps query-side persistence concerns out of Domain, Application, Contracts, and the API contract.

## Consequences

Positive:

- Product search remains server-side.
- The Product aggregate keeps value objects.
- The API contract remains unchanged.
- No database schema change is required.
- Query-specific persistence details stay in Infrastructure.

Tradeoffs:

- Catalog now has separate write and read EF models for Product.
- The read model mapping must stay aligned with `catalog.Products`.
- Future query features may need a broader read-model strategy.

## Risks

- Drift between Product aggregate mapping and `ProductSearchReadModel`.
- Runtime errors if the database schema changes without updating the read model.
- Architecture tests do not currently validate read-model mapping consistency.
- More complex future search requirements may require a formal read-model/query strategy.
