# Prompt 085: Assistant SQL Validator Executor Execution

## Date

2026-06-25

## Purpose

Execute Task 2 of the Text-to-SQL Assistant migration by adding a safe SQL validator and read-only SQL executor behind a disabled feature flag.

## Full Prompt

Implement Task 2 of the Text-to-SQL Assistant migration: add a safe SQL validator and read-only SQL executor behind a disabled feature flag. Keep the feature dormant. Do not wire Text-to-SQL into `AssistantOrchestrator`, do not add an LLM SQL planner or prompt, do not change frontend or MCP, do not add database migrations, do not change existing assistant behavior, and do not commit secrets or real read-only connection strings.

## Scope

- API-layer Text-to-SQL safety surface under `src/Api/Ecommerce.Api/Assistant/TextToSql`
- Non-secret `Assistant:TextToSql` options with `Enabled` defaulting to `false`
- Strict validator for approved assistant views only
- Read-only executor that uses separate Catalog and Orders read-only connection strings
- Architecture tests for validation, executor behavior, configuration, and dormant wiring
- Project documentation updates

## Result Summary

Added a conservative SQL validator that accepts only single `SELECT TOP (n)` queries over approved `assistant` views for the selected data source. Orders queries must include `BuyerUserId = @CurrentUserId`.

Added a read-only executor abstraction that validates before execution, selects the read-only connection by data source, injects `@CurrentUserId` for Orders queries, applies timeout and row limits, returns tabular results, and hides raw database exception text.

The feature remains disabled by default and is not called from the existing assistant endpoint. Task 3 will add the LLM planner. Task 4 will wire assistant orchestration to Text-to-SQL.
