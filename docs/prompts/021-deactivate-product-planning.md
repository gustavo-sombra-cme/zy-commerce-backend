# Prompt 021: Deactivate Product Planning

## Prompt Number

021

## Date

2026-06-08

## Purpose

Plan Deactivate Product as a DDD command feature for Catalog products.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan Deactivate Product as a DDD command feature.

Feature:
Deactivate Product

Goal:
Implement soft delete/business deactivation for Catalog products.

Also create/update:
docs/prompts/021-deactivate-product-planning.md

Do not execute.

Requirements:
- No hard delete
- Product aggregate owns the state transition
- Use CQRS command
- Use controller action
- Set IsActive to false
- Update UpdatedAt
- Return appropriate API response
- Keep read/search behavior consistent

Return:
1. Architecture Overview
2. Domain Behavior
3. CQRS Command Design
4. API Contract
5. Database Impact
6. Files Affected
7. Testing Strategy
8. Risks
9. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Deactivate Product was planned as an idempotent DDD aggregate state transition using a CQRS command, controller DELETE action, soft deactivation, and no schema changes.
