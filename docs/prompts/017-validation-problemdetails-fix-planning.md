# Prompt 017: Validation ProblemDetails Fix Planning

## Prompt Number

017

## Date

2026-06-08

## Purpose

Plan a fix for validation errors returning base ProblemDetails without the ValidationProblemDetails errors dictionary.

## Full Prompt

PLAN MODE

Using AGENT.md V2, inspect global validation error handling.

Issue:
GET /api/catalog/products?pageNumber=0&pageSize=5 returns 400 ProblemDetails without an errors dictionary.

Goal:
Validation failures must return ValidationProblemDetails including property-specific errors.

Also create/update:
docs/prompts/017-validation-problemdetails-fix-planning.md

Do not execute.

Return:
1. Root Cause Hypothesis
2. Correct Design
3. Files Affected
4. Testing Strategy
5. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

The planned fix is to serialize ValidationProblemDetails through a validation-specific writer so the errors dictionary is preserved.
