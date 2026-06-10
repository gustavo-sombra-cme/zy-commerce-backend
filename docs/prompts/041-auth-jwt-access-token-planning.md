# Prompt 041 - Auth JWT Access Token Planning

## Date

2026-06-09

## Purpose

Plan Auth Phase 5A by extending successful login to return a short-lived JWT access token without refresh tokens.

## Full Prompt

PLAN MODE

Using AGENT.md, instructions/*, PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, and NEXT_SESSION.md, plan Auth Phase 5A.

Feature:
JWT Access Token Generation

Goal:
Extend successful login to return a short-lived JWT access token, without refresh tokens yet.

Also create/update:
docs/prompts/041-auth-jwt-access-token-planning.md

Do not execute.
Do not create code yet.

Requirements:
- Add JWT generation abstraction in Auth Application
- Implement JWT generator in Auth Infrastructure
- Add JWT settings/configuration
- Update LoginUserResult to include access token metadata
- Update login response contract
- Update login endpoint response
- Configure authentication middleware only if needed for token generation/testing
- No refresh tokens
- No roles/permissions
- No Customers module
- No protected endpoints yet unless explicitly justified
- No database schema changes unless absolutely required

Return:
1. Architecture Overview
2. JWT Design
3. Configuration Design
4. Application Changes
5. Infrastructure Changes
6. API Contract Changes
7. Security Considerations
8. Files Affected
9. Testing Strategy
10. Risks
11. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned Auth Phase 5A to add JWT access token generation behind an Auth Application abstraction, implemented in Auth Infrastructure with HMAC SHA-256, 15-minute lifetime, and no refresh tokens, roles, permissions, protected endpoints, or schema changes.
