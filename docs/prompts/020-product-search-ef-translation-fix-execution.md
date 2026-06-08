# Prompt 020: Product Search EF Translation Fix Execution

## Prompt Number

020

## Date

2026-06-08

## Purpose

Execute the approved Product search EF Core translation fix.

## Full Prompt

APPROVED: EXECUTE

Execute the Product search EF translation fix exactly.

Before execution:
- create/update docs/prompts/019-product-search-ef-translation-fix-planning.md
- create docs/prompts/020-product-search-ef-translation-fix-execution.md

Fix:
- SearchProducts must not use p.Sku.Value or p.Name.Value inside EF query filtering/ordering/projection if EF cannot translate them
- Use EF.Property<string> for Sku and Name where needed
- Keep filtering server-side
- Keep DDD value objects
- Keep API contract unchanged

Do not:
- use AsEnumerable for filtering
- perform client-side search
- remove value objects
- change Domain/Application/Contracts
- create migrations unless absolutely required

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

Then manually verify:
GET /api/catalog/products?searchTerm=Product&pageNumber=1&pageSize=5

Report:
- files changed
- whether migration was avoided
- test results
- manual verification result
- architecture test result
- deviations

## Status

EXECUTED

## Result Summary

Execution started by creating the required prompt logs before infrastructure query changes.
