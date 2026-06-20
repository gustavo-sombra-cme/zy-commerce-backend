# Prompt 023: Auth Module Planning

## Prompt Number

023

## Date

2026-06-08

## Purpose

Plan introduction of the Auth module before login or registration features are implemented.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan introduction of the Auth module.

Goal:
Establish authentication architecture before any login or registration features are implemented.

Also create/update:
docs/prompts/023-auth-module-planning.md

Do not execute.

Topics to cover:
1. Module boundaries
2. Auth vs Customers responsibilities
3. JWT strategy
4. Refresh token strategy
5. User aggregate design
6. Database ownership
7. CQRS structure
8. Project structure changes
9. Risks
10. Execution roadmap

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Auth was planned as an isolated module with Domain, Application, Infrastructure, Contracts, and UnitTests projects. Phase 1 is skeleton only, with no User aggregate, JWT, refresh tokens, password hashing, DbContext, migrations, API endpoints, Customers module, roles, or permissions.
