# Prompt 031 - Auth User Aggregate Planning

## Date

2026-06-09

## Purpose

Plan Auth Phase 2 by designing the Auth User aggregate before registration, login, JWT, refresh tokens, or persistence are introduced.

## Full Prompt

PLAN MODE

Using AGENT.md, instructions/*, PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, and NEXT_SESSION.md, plan Auth Phase 2.

Feature:
Auth User Aggregate

Goal:
Design the Auth domain model before implementing registration, login, JWT, refresh tokens, or persistence.

Also create/update:
docs/prompts/031-auth-user-aggregate-planning.md

Do not execute.
Do not create code yet.

Requirements:
- Auth remains separate from Customers
- No API endpoints
- No JWT implementation
- No password hashing implementation
- No DbContext
- No migrations
- No roles/permissions yet
- Domain must not depend on infrastructure libraries
- Use DDD value objects

Return:
1. Architecture Overview
2. Auth vs Customers Boundary
3. User Aggregate Design
4. Value Objects
5. Domain Behaviors
6. Domain Rules/Invariants
7. Files Affected
8. Testing Strategy
9. Risks
10. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned an Auth domain-only User aggregate with UserId, Email, and PasswordHash value objects, Auth unit tests, and no API, Application, Infrastructure, persistence, JWT, refresh token, role, permission, or Customers work.
