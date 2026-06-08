# Prompt 009: Database Migrations Planning

## Prompt Number

009

## Date

2026-06-08

## Purpose

Plan database initialization for the Catalog module using EF Core migrations and a local SQL Server database.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan database initialization for the Catalog module.

Goal:
Introduce EF Core migrations and a local SQL Server database.

Also create/update:
docs/prompts/009-database-migrations-planning.md

Do not execute.

Return:
1. Architecture Overview
2. Migration Strategy
3. Database Lifecycle
4. Local Development Strategy
5. Files Affected
6. Risks
7. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Database initialization was planned for the Catalog module using manual EF Core migrations, SQL Server LocalDB, the ConnectionStrings:Catalog configuration key, and the InitialCatalogSchema migration in the Infrastructure project.
