# Prompt 033 - Auth Register User Planning

## Date

2026-06-09

## Purpose

Plan Auth Phase 3A by implementing Register User in the Auth Application layer only.

## Full Prompt

PLAN MODE

Revise the Register User plan by splitting it into smaller phases.

Goal:
Avoid implementing Application, Infrastructure, API, and migration all in one execution.

Create two phases:

Phase 3A:
Register User Application Layer only.

Allowed:
- RegisterUserCommand
- RegisterUserCommandHandler
- RegisterUserCommandValidator
- RegisterUserResult
- DuplicateEmailException
- IUserRepository abstraction
- IAuthUnitOfWork abstraction
- IPasswordHasher abstraction
- Auth Application DI if needed
- Unit tests using fakes

Forbidden:
- AuthDbContext
- EF configuration
- migrations
- API controller
- Contracts
- PasswordHasher implementation
- JWT
- login
- refresh tokens
- roles/permissions
- Customers module

Phase 3B:
Auth Persistence + API registration endpoint.

Return the revised plan for Phase 3A only.

Also create/update:
docs/prompts/033-auth-register-user-planning.md

Do not execute.

Return:
1. Architecture Overview
2. Registration Application Flow
3. CQRS Command Design
4. Abstractions
5. Files Affected
6. Testing Strategy
7. Risks
8. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned Phase 3A as an Auth Application-only Register User use case with CQRS, FluentValidation, repository/unit-of-work/password-hashing abstractions, and fake-based unit tests. Infrastructure, API, contracts, migrations, JWT, login, refresh tokens, roles, permissions, and Customers remained out of scope.
