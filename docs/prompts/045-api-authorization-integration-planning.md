# Prompt 045 - API Authorization Integration Planning

Date: 2026-06-09

## Purpose

Plan Swagger JWT authorization and Catalog write endpoint protection.

## Full Prompt

PLAN MODE

Using AGENT.md, instructions/*, PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, and NEXT_SESSION.md, plan API authorization integration.

Feature:
Swagger JWT Authorization + Protect Catalog Write Endpoints

Goal:
Allow Swagger users to authenticate with JWT access tokens and protect Catalog write endpoints.

Also create/update:
docs/prompts/045-api-authorization-integration-planning.md

Do not execute.
Do not create code yet.

Requirements:
- Add Swagger/OpenAPI bearer token security definition
- Swagger UI must show Authorize button
- User can paste JWT access token
- Protect Catalog write endpoints with [Authorize]
- Keep Catalog read endpoints public
- Do not add roles/permissions yet
- Do not add refresh tokens
- Do not add Customers module
- Do not change Auth domain
- Do not create migrations
- No database schema changes

Return:
1. Architecture Overview
2. Swagger JWT Design
3. Catalog Authorization Design
4. Endpoint Access Matrix
5. Files Affected
6. Testing Strategy
7. Risks
8. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned API-only authorization integration: Swagger bearer token support, `[Authorize]` on Catalog write endpoints only, public Catalog reads, and authorization expectation tests.
