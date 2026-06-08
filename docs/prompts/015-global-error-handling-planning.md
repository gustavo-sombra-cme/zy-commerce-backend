# Prompt 015: Global Error Handling Planning

## Prompt Number

015

## Date

2026-06-08

## Purpose

Plan consistent global API error responses across Catalog endpoints.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan global API error handling.

Goal:
Introduce consistent API error responses across all Catalog endpoints.

Requirements:
- ProblemDetails
- Global exception middleware
- Validation handling
- NotFound handling
- Conflict handling
- Generic server error handling

Do not execute.

Return:
1. Architecture Overview
2. Error Handling Design
3. Error Response Contract
4. Files Affected
5. Testing Strategy
6. Risks
7. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Global API error handling was planned as API-layer middleware using ProblemDetails and ValidationProblemDetails for validation, conflict, not found, and generic server errors.
