# Prompt 102: Inventory Reservation Planning

## Prompt Number

102

## Date

2026-07-22

## Purpose

Plan the first Inventory Reservation vertical slice as a new ecommerce bounded context while preserving Catalog and Orders ownership, preventing over-reservation under concurrency, and defining CQRS, persistence, API, migration, event, authorization, testing, documentation, and ADR impacts without implementing the feature.

## Full Prompt

`PLAN NEXT ECOMMERCE FEATURE: INVENTORY RESERVATION`

Act as a senior DDD, Clean Architecture, CQRS, and ecommerce architect. Use repository-local conditional-loading rules and evaluate at least `architecture-decision-check`, `migration-safety-check`, and `prompt-log-writer`. Inspect current source rather than relying only on documentation, search existing ADRs, and report observable Skill and instruction-loading traces without exposing hidden reasoning.

Plan an Inventory module that owns stock and reservations for Catalog products and can reserve stock for an Order before payment and fulfillment. The first phase must cover inventory records, stock recording, all-or-nothing multi-line reservation, release and commit domain behavior, over-reservation prevention, concurrent requests, idempotency, reservation status and expiration modeling, relevant events, clear Catalog/Orders/Inventory boundaries, CQRS commands and queries, application services, persistence, necessary contracts/endpoints, authorization, SQL Server/EF Core concurrency, transaction and Unit of Work behavior, migration impact, explicit errors, tests, documentation, risks, deferred work, and an actionable execution checklist.

Evaluate whether Inventory is a separate bounded context; whether it owns stock quantities; whether Catalog remains descriptive; whether Orders integrates synchronously, by HTTP, asynchronously, or through a saga; whether an Outbox is required; the source of truth; concurrency and transaction strategy; duplicate/failure handling; and expiration cleanup. Compare reservation aggregate options and define identities, invariants, state transitions, quantities, events, persistence ownership, idempotency keys, lock ordering, rollback behavior, event timing, APIs, policies, tables, indexes, foreign keys, and test coverage. Prefer the simplest approach compatible with the current modular monolith and future payment evolution.

This is planning only. Do not implement code, modify source or project-memory files, create or apply migrations, execute database commands, install packages, or run build/test commands. The only permitted repository artifact is this next chronological planning log. Report migration plan safety with execution approval `NOT_REQUESTED`; identify the ADR action without creating or editing an ADR. Return every heading required by the attached request, perform its stated self-validation, and end exactly with `PLAN_STATUS: PENDING_APPROVAL`.

## Status

PLANNED

## Result Summary

Completed a repository-evidenced plan for a separate Inventory bounded context, an all-or-nothing multi-line reservation transaction, SQL Server `rowversion` optimistic concurrency, deterministic idempotency, an Orders-owned submit workflow through an API composition-root adapter, explicit expiration modeling without a background worker, additive Inventory and Orders migrations, and domain-event recording without an event bus or Outbox in phase one.

The architecture review found no existing ADR that owns Inventory boundaries, cross-database coordination, or reservation concurrency. The result is `ADR_ACTION: CREATE`; ADR-002 remains authoritative for historical Orders product snapshots and is not replaced. The migration planning result is `MIGRATION_PLAN_SAFETY: PASS`, with `MIGRATION_EXECUTION_APPROVAL: NOT_REQUESTED`; no migration was created or applied.

Invoked Skills: `.agents/skills/prompt-log-writer/SKILL.md`, `.agents/skills/architecture-decision-check/SKILL.md`, and `.agents/skills/migration-safety-check/SKILL.md`. Loaded routing and instruction sources: `AGENT.md`, `instructions/00-role-and-stack.md`, `instructions/01-execution-and-planning.md`, `instructions/02-architecture-and-modules.md`, `instructions/03-cqrs-database-testing-security.md`, `instructions/04-documentation-and-memory.md`, `instructions/06-loading-index.md`, `docs/project/PROMPT_TEMPLATE.md`, and `docs/agents/workflow/planning-sub-agent.md`. Relevant project memory, ADR-002, current Catalog/Orders/API source, persistence mappings, migration conventions, authorization/error handling, project references, and architecture tests were inspected conditionally.

Only this planning prompt log was created. No implementation or source file was modified, no project or package was added, no migration or database command was executed, and no build or test command was run.
