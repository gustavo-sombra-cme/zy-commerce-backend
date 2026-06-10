# Prompt 038 - Auth Login Application Execution

## Date

2026-06-09

## Purpose

Execute Auth Phase 4A: Login User Application Layer.

## Full Prompt

APPROVED: EXECUTE

Execute Auth Phase 4A: Login User Application Layer.

Before execution:
- create/update docs/prompts/037-auth-login-application-planning.md
- create docs/prompts/038-auth-login-application-execution.md

Implement:
- LoginUserCommand
- LoginUserCommandHandler
- LoginUserCommandValidator
- LoginUserResult
- InvalidCredentialsException
- InactiveUserException
- IUserRepository.GetByEmailAsync abstraction
- IPasswordHasher verification method
- fake-based Auth unit tests

Allowed infrastructure change:
- Update existing Auth Infrastructure PasswordHasher only to implement the new verification method required by IPasswordHasher.
- Do not change AuthDbContext, UserRepository, EF configuration, migrations, DI, or API.

Do not create:
- API endpoint
- Contracts
- JWT
- refresh tokens
- login HTTP route
- roles/permissions
- Customers module
- migrations
- DbContext changes
- repository implementation changes

Update:
- PROJECT_STATUS.md
- AI_HANDOFF.md
- ROADMAP.md
- NEXT_SESSION.md

Run:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

Report:
- files changed
- tests added
- test results
- architecture test result
- memory docs updated
- package changes if any
- deviations

## Status

EXECUTED

## Result Summary

Implemented Auth Login User Application workflow with CQRS command, validator, handler, result, invalid credentials and inactive user exceptions, password verification abstraction, fake-based unit tests, and the required Infrastructure password hasher verification method.
