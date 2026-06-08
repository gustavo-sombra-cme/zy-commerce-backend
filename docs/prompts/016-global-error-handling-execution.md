# Prompt 016: Global Error Handling Execution

## Prompt Number

016

## Date

2026-06-08

## Purpose

Execute the approved global API error handling plan.

## Full Prompt

APPROVED: EXECUTE

Execute the global API error handling plan exactly.

Before execution:
- create/update docs/prompts/015-global-error-handling-planning.md
- create docs/prompts/016-global-error-handling-execution.md

Add:
- ExceptionHandlingMiddleware in Ecommerce.Api
- ProblemDetails / ValidationProblemDetails responses
- ValidationException => 400
- DuplicateSkuException => 409
- NotFound support => 404
- Unknown exception => 500 generic message

Update:
- Program.cs to register middleware
- ProductsController to remove local ValidationException / DuplicateSkuException catch blocks

Keep:
- explicit Guid.Empty => 400
- query null result => 404

Do not modify:
- Catalog Domain
- Catalog Application
- Catalog Infrastructure
- Catalog Contracts
- migrations
- modules
- projects

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

Report:
- files changed
- test results
- architecture test result
- deviations

## Status

EXECUTED

## Result Summary

Execution started by creating the required prompt logs before API middleware changes.
