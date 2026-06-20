# Prompt 003: Architecture Tests Planning

## Prompt Number

003

## Date

2026-06-08

## Purpose

Plan architecture tests that protect Clean Architecture dependency rules before business features are added.

## Full Prompt

PLAN MODE

Using AGENT.md V2 and the current solution, plan architecture tests only.

Goal:
Protect Clean Architecture dependency rules before adding business features.

Also create/update the prompt log:
docs/prompts/003-architecture-tests-planning.md

Do not execute.
Do not create test code yet.

Plan tests that verify:
- Domain projects do not reference Application, Infrastructure, Contracts, or Api
- Application projects do not reference Infrastructure or Api
- Infrastructure projects do not reference Api
- BuildingBlocks projects do not reference Catalog projects
- Only approved Day 1 projects exist
- No Bootstrapper or Shared projects exist

Return:
1. Architecture Overview
2. Test Library Choice
3. Test Categories
4. Files Affected
5. Rules Enforced
6. Risks
7. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Architecture tests were planned to enforce Clean Architecture dependency direction, BuildingBlocks isolation, approved Day 1 project boundaries, and forbidden Bootstrapper/Shared projects.
