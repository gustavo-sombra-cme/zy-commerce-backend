# Prompt 087: Assistant Text-to-SQL Orchestration Execution

Date: 2026-06-25

## Purpose

Execute Task 4 of the Text-to-SQL Assistant migration: wire the live assistant orchestrator to optionally use Text-to-SQL behind the existing disabled-by-default feature flag.

## Approval Status

APPROVED: EXECUTE

## Prompt Summary

Repository: `C:\ZippyYum\Learning\zy-commerce-backend`

Branch: `feature/backend-assistant-text-to-sql-orchestration`

Requested work:

- Start from `main`, confirm a clean tree, fetch/pull, confirm Tasks 1 through 3 are merged, then create the Task 4 branch.
- Wire `AssistantOrchestrator` to use the Text-to-SQL planner, validator, read-only executor, and mapper only when `Assistant:TextToSql:Enabled` is true.
- Preserve existing assistant behavior when the feature flag is disabled.
- Preserve existing assistant behavior as fallback when Text-to-SQL planning, validation, execution, or mapping fails safely.
- Keep the existing `AssistantQueryResponse` contract stable.
- Do not expose generated SQL, raw provider responses, raw database errors, connection strings, JWTs, or secrets.
- Do not add frontend, MCP, database schema, migrations, admin tools, write behavior, generic table public response support, provider SDKs, committed secrets, or live provider/database tests.
- Do not push.

## Execution Notes

- Text-to-SQL is a first-pass path only when explicitly enabled.
- Candidate SQL remains untrusted and must pass `AssistantSqlValidator`.
- Orders execution uses backend-authenticated `buyerId` as `@CurrentUserId`.
- Catalog execution uses the catalog read-only source and public catalog data scope.
- `genericTable` remains internal/unexposed and falls back to the existing assistant path.
- Existing assistant flow remains fallback for safe Text-to-SQL failure modes.

## Result Summary

Implemented feature-flagged orchestration with a mapper to existing assistant response shapes, tests, and project documentation updates. Full verification and commit details are recorded in the assistant execution response for this task.
