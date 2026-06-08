# Prompt 005: Catalog Create Product Planning

## Prompt Number

005

## Date

2026-06-08

## Purpose

Plan the first Catalog business feature, Create Product, using Clean Architecture and CQRS.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan the first Catalog business feature.

Feature:
Create Product

Also create/update:
docs/prompts/005-catalog-create-product-planning.md

Do not execute.
Do not create code yet.

Design using Clean Architecture and CQRS.

Return:
1. Architecture Overview
2. Domain Design
3. CQRS Design
4. Validation Rules
5. Database Design
6. API Contract
7. Files Affected
8. Testing Strategy
9. Risks
10. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

Revision decisions:

1. Use Controllers, not Minimal APIs.
2. API file should be:
   src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs
3. Do not create database migrations.
4. Do not include price, inventory, category, images, variants, or SEO.
5. Keep Product as the first Catalog aggregate only.
6. Keep prompt logging enabled.

## Status

APPROVED

## Result Summary

Create Product was planned as the first Catalog aggregate feature using controllers, CQRS with MediatR, FluentValidation, EF Core model/configuration only, and no database migrations.
