# Prompt 039 - Auth Login Persistence API Planning

## Date

2026-06-09

## Purpose

Plan Auth Phase 4B by wiring the existing LoginUserCommand to real Auth persistence and an HTTP endpoint without issuing JWT or refresh tokens.

## Full Prompt

PLAN MODE

Using AGENT.md, instructions/*, PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, and NEXT_SESSION.md, plan Auth Phase 4B.

Feature:
Login User Persistence + API Endpoint

Goal:
Wire the existing LoginUserCommand to real Auth persistence and an HTTP endpoint, but still do not issue JWT or refresh tokens.

Also create/update:
docs/prompts/039-auth-login-persistence-api-planning.md

Do not execute.
Do not create code yet.

Requirements:
- Implement IUserRepository.GetByEmailAsync in Auth Infrastructure UserRepository
- Add Auth Contracts LoginUserRequest/LoginUserResponse
- Add Auth API login endpoint
- Use existing PasswordHasher.Verify
- Return userId and email only
- Return 401 Unauthorized for invalid credentials
- Return 403 Forbidden or 409 Conflict for inactive user, choose and explain
- No JWT
- No refresh tokens
- No roles/permissions
- No Customers module
- No migrations unless absolutely required and explained
- No DbContext schema changes unless absolutely required and explained

Return:
1. Architecture Overview
2. Login Persistence Flow
3. API Contract
4. Error Handling Design
5. Files Affected
6. Testing Strategy
7. Risks
8. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned Auth Phase 4B with repository email lookup implementation, login contracts, login endpoint, 401 invalid credentials handling, 403 inactive user handling, no schema changes, no migration, and no token behavior.
