# Prompt 070: Catalog Product Price Write Support Planning

## Prompt Number

070

## Date

2026-06-18

## Purpose

Plan Catalog Product Price Write Support as a Domain Feature.

## Full Prompt

Plan backend feature: Catalog Product Price Write Support

Goal:
Allow product price to be set from Catalog write APIs so product search/details no longer always return price = 0.

Use docs/project/PROMPT_TEMPLATE.md exactly.
Classify this as a Domain Feature.

Context:
- Catalog product read responses already return price.
- Current Create Product and Update Product Details requests do not include price.
- Because price is never set through write APIs, frontend receives price = 0.
- Frontend cart/checkout depends on Catalog price for display snapshots and order snapshot fields.

Scope:
- Add price to CreateProductRequest and CreateProductCommand if missing.
- Ensure Product aggregate creation stores price.
- Add validation for price, likely price >= 0 or price > 0 depending existing domain rules.
- Add/update tests for create product with price.
- Decide whether price update belongs in:
  - a separate UpdateProductPrice command, preferred, or
  - existing UpdateProductDetails, only if approved by plan.
- Update product search/details contracts if needed.
- Update Swagger/manual verification docs.
- Update frontend contract docs.

Out of Scope:
- Inventory
- Discounts
- Coupons
- Tax
- Currency conversion
- Payments
- Shipping
- MCP changes
- Frontend changes, except documentation if needed

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned Catalog-owned product price write support through create product only, with price stored on the Product aggregate, validated as non-negative, persisted through EF Core with a Catalog migration, returned from search/details APIs, and documented for frontend cart/checkout usage. Price update commands/endpoints, currency support, discounts, coupons, and MCP changes were excluded.
