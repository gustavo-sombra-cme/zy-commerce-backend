# ADR-002: Orders Product Snapshot Strategy

## Date

2026-06-12

## Status

Accepted

## Context

The Orders module is being introduced as an initial vertical slice before MCP integration. The slice supports Create Order and Get Order By Id only. Orders must remain isolated from Catalog and Auth internals, and the slice does not include inventory reservation, payments, shipping, discounts, coupons, cancellation, refunds, advanced order workflows, or a Customer profile module.

Order history must preserve what the buyer ordered at the time the order was created. Catalog product names, SKUs, and prices can change later, so Orders cannot depend on live Catalog state for historical order details.

## Options Considered

1. Reference Catalog internals directly from Orders.

   Rejected because it violates module isolation and creates direct coupling between Orders and Catalog implementation details.

2. Query Catalog through an integration mechanism during order creation.

   Rejected for this initial slice because no cross-module integration mechanism has been approved yet. This can be added later as an explicit integration phase.

3. Store only product ids in Orders.

   Rejected because order history would not preserve the SKU, name, and price used at purchase time.

4. Store product snapshot data supplied by the create request.

   Accepted for the initial slice.

## Decision

Orders will store product snapshot data on each order line. For this initial slice, the create request supplies the snapshot data:

- product id
- SKU
- product name
- unit price
- quantity

The Order aggregate stores this data as part of its own persistence model and calculates totals from the captured line snapshots.

## Rationale

This keeps Orders independent from Catalog internals while preserving historical order details. It also allows the initial Orders vertical slice to ship with clear module boundaries and without inventing an unapproved integration mechanism.

The strategy aligns with DDD ownership: Orders owns order history and order line facts, while Catalog owns product lifecycle and product management.

## Consequences

Positive:

- Orders remains isolated from Catalog.
- Historical order details are stable even when Catalog changes later.
- No cross-module reference is required.
- Future MCP exposure can read complete order details from Orders without reaching into Catalog.

Tradeoffs:

- Product snapshot data can be spoofed until a Catalog validation/integration phase is approved.
- Later Catalog integration must preserve the snapshot boundary instead of replacing historical data with live product data.
- Orders must duplicate product display data intentionally as historical facts.

## Risks

- Invalid product snapshots may be submitted by clients in this initial slice.
- Future integrations could accidentally reintroduce direct Catalog coupling.
- Pricing validation is deferred until a dedicated Catalog/pricing integration phase.
