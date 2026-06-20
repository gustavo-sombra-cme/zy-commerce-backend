# Prompt 066 - Orders Initial Vertical Slice Execution

## Prompt Number

066

## Date

2026-06-12

## Purpose

Execute the approved Orders Initial Vertical Slice.

## Full Prompt

APPROVED: EXECUTE Orders Initial Vertical Slice

Additional Explicit Approvals:
- Approved to create the Orders module.
- Approved to create Orders Domain, Application, Infrastructure, Contracts, and UnitTests projects.
- Approved to create the initial Orders EF Core migration and Orders database schema.
- Approved to add Orders API endpoints:
  - POST /api/orders
  - GET /api/orders/{orderId}
- Approved to add an ADR for Orders product snapshot strategy.

Additional Requirements:
- Scope is Create Order and Get Order By Id only.
- Orders must not reference Catalog or Auth internals.
- Use product snapshot data in the create request for this slice.
- Do not add payments, inventory, shipping, discounts, coupons, refunds, cancellation, or advanced order workflows.
- Both endpoints require bearer authentication.
- Users may only retrieve their own orders.
- Keep API controllers thin.
- Keep Product snapshot strategy documented.

## Status

EXECUTED

## Result Summary

Implemented the Orders Initial Vertical Slice as a new approved module with Domain, Application, Infrastructure, Contracts, and UnitTests projects. Added the `Order` aggregate, `OrderLine` snapshot design, Create Order and Get Order By Id CQRS flows, EF Core persistence, initial Orders migration, protected API endpoints `POST /api/orders` and `GET /api/orders/{orderId}`, and ADR-002 for the product snapshot strategy. Orders does not reference Catalog or Auth internals. The local `EcommerceOrders` database was created and updated through `InitialOrdersSchema`. `dotnet restore Ecommerce.sln`, `dotnet build Ecommerce.sln`, and `dotnet test Ecommerce.sln` passed with 70 Catalog unit tests, 65 Auth unit tests, 12 Orders unit tests, and 33 architecture tests. Manual API smoke verification passed for unauthorized create, authenticated create, same-user get, cross-user get returning `404 NotFound`, and correlation header preservation.
