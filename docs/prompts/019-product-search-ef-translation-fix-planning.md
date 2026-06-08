# Prompt 019: Product Search EF Translation Fix Planning

## Prompt Number

019

## Date

2026-06-08

## Purpose

Plan a fix for Product search EF Core translation while preserving DDD value objects.

## Full Prompt

PLAN MODE

Using AGENT.md V2, plan a fix for Product search EF translation.

Issue:
SearchProducts causes InvalidOperationException because EF Core cannot translate EF.Functions.Like over value object properties:
p.Sku.Value
p.Name.Value

Goal:
Keep DDD value objects, but make EF Core search translate to SQL.

Also create/update:
docs/prompts/019-product-search-ef-translation-fix-planning.md

Do not execute.

Rules:
- Do not use AsEnumerable for filtering.
- Do not perform client-side search.
- Do not remove value objects from the domain.
- Do not change API contract.
- Do not create migrations unless absolutely required and explained.

Return:
1. Root Cause
2. Correct Design Options
3. Recommended Fix
4. Files Affected
5. Testing Strategy
6. Risks
7. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

The approved plan is to keep value objects in the domain and use EF.Property<string> for Sku and Name in server-side search filtering, ordering, and projection.
