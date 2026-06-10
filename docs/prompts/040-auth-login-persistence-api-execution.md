# Prompt 040 - Auth Login Persistence API Execution

## Date

2026-06-09

## Purpose

Execute Auth Phase 4B: Login User Persistence + API Endpoint.

## Full Prompt

APPROVED: EXECUTE

Execute Auth Phase 4B: Login User Persistence + API Endpoint.

Before execution:
- create/update docs/prompts/039-auth-login-persistence-api-planning.md
- create docs/prompts/040-auth-login-persistence-api-execution.md

Implement:
- UserRepository.GetByEmailAsync
- LoginUserRequest
- LoginUserResponse
- POST /api/auth/users/login action
- InvalidCredentialsException => 401 Unauthorized
- InactiveUserException => 403 Forbidden

Use:
- existing PasswordHasher.Verify
- existing AuthDbContext
- existing LoginUserCommand
- existing LoginUserCommandHandler
- existing LoginUserCommandValidator
- response with userId and email only

Do not create:
- JWT
- refresh tokens
- roles/permissions
- Customers module
- migrations
- DbContext schema changes
- login timestamp persistence

Update:
- PROJECT_STATUS.md
- AI_HANDOFF.md
- ROADMAP.md
- NEXT_SESSION.md

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

Manual verify:
- POST /api/auth/users/register returns 201 Created
- POST /api/auth/users/login with correct password returns 200 OK
- login response contains only userId and email
- login with wrong password returns 401 Unauthorized
- inactive user returns 403 Forbidden if feasible without extra API/schema changes

Report:
- files changed
- test results
- architecture test result
- migration status
- manual verification result
- memory docs updated
- deviations

## Status

EXECUTED

## Result Summary

Implemented the Auth login repository lookup, contracts, API endpoint, and exception mappings. Verified registration, successful login, wrong-password 401, and no token fields; inactive-user verification was not feasible without extra API or schema changes.
