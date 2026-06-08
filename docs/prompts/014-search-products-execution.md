# Prompt 014: Search Products Execution

## Prompt Number

014

## Date

2026-06-08

## Purpose

Execute the approved Search/List Products query feature.

## Full Prompt

APPROVED: EXECUTE

Execute the Search/List Products plan exactly.

Before execution:
- create/update docs/prompts/013-search-products-planning.md
- create docs/prompts/014-search-products-execution.md

Use:
- Query side of CQRS
- Controller GET action
- Pagination
- Search by SKU or Name
- Optional IsActive filter
- AsNoTracking EF projection

Do not create:
- domain changes
- migrations
- new modules
- new projects
- write behavior

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

Execution started by creating the required prompt log before query-side code changes.
