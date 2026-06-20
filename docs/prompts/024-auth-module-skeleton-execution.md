# Prompt 024: Auth Module Skeleton Execution

## Prompt Number

024

## Date

2026-06-08

## Purpose

Execute Phase 1 of the Auth module plan by creating the Auth module skeleton.

## Full Prompt

APPROVED: EXECUTE

Execute Phase 1 only: Auth module skeleton.

Before execution:
- create/update docs/prompts/023-auth-module-planning.md
- create docs/prompts/024-auth-module-skeleton-execution.md

Create:
- src/Modules/Auth/Ecommerce.Auth.Domain
- src/Modules/Auth/Ecommerce.Auth.Application
- src/Modules/Auth/Ecommerce.Auth.Infrastructure
- src/Modules/Auth/Ecommerce.Auth.Contracts
- tests/UnitTests/Ecommerce.Auth.UnitTests

Update:
- Ecommerce.sln
- project references using Clean Architecture
- architecture tests to allow Auth
- architecture tests to enforce Catalog/Auth isolation
- architecture tests to ensure BuildingBlocks does not reference Auth

Do not create:
- User aggregate
- JWT
- refresh tokens
- password hashing
- DbContext
- migrations
- API endpoints
- Customers module
- roles/permissions

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

Report:
- files changed
- projects created
- references added
- architecture test result
- total test result
- deviations

## Status

EXECUTED

## Result Summary

Execution started by creating the required prompt logs before scaffolding the Auth skeleton.
