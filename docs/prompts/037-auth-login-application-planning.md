# Prompt 037 - Auth Login Application Planning

## Date

2026-06-09

## Purpose

Plan Auth Phase 4A by adding the Login User Application workflow without JWT, refresh tokens, or an API endpoint.

## Full Prompt

PLAN MODE

Using AGENT.md, instructions/*, PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, and NEXT_SESSION.md, plan Auth Phase 4A.

Feature:
Login User Application Layer

Goal:
Implement the application workflow for user login without JWT, refresh tokens, or API endpoint.

Also create/update:
docs/prompts/037-auth-login-application-planning.md

Do not execute.
Do not create code yet.

Requirements:
- Use CQRS command
- Use FluentValidation
- Use existing User aggregate
- Use existing Email value object
- Check user exists by email
- Verify password through an Application abstraction
- Reject inactive users
- Record login timestamp only if appropriate for this phase
- No JWT
- No refresh tokens
- No API endpoint
- No AuthDbContext changes unless repository abstraction requires planning only
- No migrations
- No roles/permissions
- No Customers module

Return:
1. Architecture Overview
2. Login Application Flow
3. CQRS Command Design
4. Password Verification Design
5. Repository Changes
6. Domain Behavior Impact
7. Files Affected
8. Testing Strategy
9. Risks
10. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned Auth Phase 4A as an Application-layer login workflow with CQRS, FluentValidation, password verification abstraction, repository lookup abstraction, fake-based unit tests, no API endpoint, no JWT, no refresh tokens, no migrations, and no persistence model changes.
