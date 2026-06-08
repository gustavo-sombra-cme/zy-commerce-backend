# Prompt 007: Get Product By Id Planning

## Prompt Number

007

## Date

2026-06-08

## Purpose

Plan the first Catalog query feature to introduce the read side of CQRS.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan the first Catalog query feature.

Feature:
Get Product By Id

Goal:
Introduce the Query side of CQRS.

Also create/update:
docs/prompts/007-get-product-by-id-planning.md

Do not execute.
Do not create code yet.

Return:
1. Architecture Overview
2. CQRS Query Design
3. API Contract
4. Files Affected
5. Testing Strategy
6. Risks
7. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Get Product By Id was planned as the first Catalog query-side CQRS feature using a controller action, read repository, DTO projection, 404 for missing products, and 400 for empty product ids.
