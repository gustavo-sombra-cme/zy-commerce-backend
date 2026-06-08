# Prompt 022: Deactivate Product Execution

## Prompt Number

022

## Date

2026-06-08

## Purpose

Execute the approved Deactivate Product command feature.

## Full Prompt

APPROVED: EXECUTE

Execute the Deactivate Product plan exactly.

Before execution:
- create/update docs/prompts/021-deactivate-product-planning.md
- create docs/prompts/022-deactivate-product-execution.md

Use:
- DDD aggregate behavior: Product.Deactivate(...)
- CQRS command
- DeactivateProductCommandValidator
- DeactivateProductCommandHandler
- DELETE /api/catalog/products/{productId}
- 204 No Content on success
- idempotent deactivation
- no hard delete

Do not create:
- migrations
- new modules
- new projects
- contracts unless needed
- hard delete logic
- domain events

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

Then manually verify in Swagger:
1. Create product
2. DELETE product
3. GET product by id
4. Confirm isActive=false and updatedAt is set
5. Search with isActive=false

Report:
- files changed
- test results
- architecture test result
- migration status
- manual verification result
- deviations

## Status

EXECUTED

## Result Summary

Execution started by creating the required prompt logs before code changes.
