# Prompt 035 - Auth Register User Persistence API Planning

## Date

2026-06-09

## Purpose

Plan Auth Phase 3B by wiring the existing RegisterUser application command to real infrastructure and an HTTP endpoint.

## Full Prompt

PLAN MODE

Using AGENT.md, instructions/*, PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, and NEXT_SESSION.md, plan Auth Phase 3B.

Feature:
Register User Persistence + API Endpoint

Goal:
Wire the existing RegisterUser application command to real infrastructure and an HTTP endpoint.

Also create/update:
docs/prompts/035-auth-register-user-persistence-api-planning.md

Do not execute.
Do not create code yet.

Requirements:
- Implement IPasswordHasher in Auth Infrastructure
- Implement IUserRepository in Auth Infrastructure
- Implement IAuthUnitOfWork
- Add AuthDbContext
- Add EF Core User configuration
- Add ConnectionStrings:Auth
- Add manual Auth migration
- Add Auth Contracts request/response
- Add Auth API controller endpoint
- Register Auth Application and Infrastructure in Program.cs
- Return 201 Created on success
- Return 409 Conflict for duplicate email
- No JWT
- No login
- No refresh tokens
- No roles/permissions
- No Customers module
- No cross-module references

Return:
1. Architecture Overview
2. Persistence Design
3. Password Hashing Implementation
4. API Contract
5. Migration Strategy
6. Dependency Registration
7. Files Affected
8. Testing Strategy
9. Risks
10. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned Auth Phase 3B with Auth persistence, password hashing implementation, contracts, API endpoint, DI registration, manual migration, LocalDB update, and duplicate-email conflict handling. JWT, login, refresh tokens, roles, permissions, Customers, cross-module references, and startup auto-migration remained out of scope.
