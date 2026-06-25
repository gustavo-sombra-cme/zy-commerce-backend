# Prompt 086: Assistant Text-to-SQL Planner Execution

Date: 2026-06-25

## Purpose

Execute Task 3 of the Text-to-SQL Assistant migration: add a dormant LLM planner that produces untrusted candidate SQL plans for the already-approved assistant read-only views.

## Approval Status

APPROVED: EXECUTE

## Prompt Summary

Repository: `C:\ZippyYum\Learning\zy-commerce-backend`

Branch: `feature/backend-assistant-text-to-sql-planner`

Requested work:

- Start from `main`, confirm a clean tree, fetch/pull, confirm Tasks 1 and 2 are merged, then create the Task 3 branch.
- Add backend-only Text-to-SQL planner code under `src/Api/Ecommerce.Api/Assistant/TextToSql`.
- Reuse the existing assistant LLM client abstraction.
- Add prompt builder, JSON plan contract, fail-closed parser, planner service, DI registration, tests, and documentation.
- Keep the feature dormant.
- Do not wire Text-to-SQL into `AssistantOrchestrator`.
- Do not execute generated SQL from the live assistant.
- Do not add frontend, MCP, database schema, migrations, provider SDKs, secrets, or live provider tests.
- Do not push.

## Execution Notes

- Task 3 planner output is untrusted and must pass the Task 2 SQL validator before any future execution.
- The prompt contains only approved assistant views and actual columns.
- The prompt intentionally omits nonexistent columns such as `CurrencyCode`.
- Existing `POST /api/assistant/query` behavior remains unchanged.

## Result Summary

Implemented as a dormant planning layer with tests and project documentation updates. Full verification and commit details are recorded in the assistant execution response for this task.
