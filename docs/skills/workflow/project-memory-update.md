# project-memory-update

## Purpose

Keep project memory factual after completed execution tasks.

## When to use

Use when a task changes project state, operating constraints, workflow rules, architecture, APIs, packages, tests, or documentation structure.

## Inputs

* Completed task summary.
* Changed files.
* Verification results.

## Outputs

* Updated project memory files or a reason no update was required.

## Required reading

* `instructions/04-documentation-and-memory.md`
* `docs/project/PROJECT_STATUS.md`
* `docs/project/AI_HANDOFF.md`
* `docs/project/ROADMAP.md`
* `docs/project/NEXT_SESSION.md`

## Files it may read

* `docs/project/*`
* Prompt logs.
* ADRs.

## Files it may update

* `docs/project/PROJECT_STATUS.md`
* `docs/project/AI_HANDOFF.md`
* `docs/project/ROADMAP.md`
* `docs/project/NEXT_SESSION.md`

## Stop conditions

* Proposed update records speculative work as complete.
* Project memory conflicts with actual repo state.

## Verification expectations

Memory updates must be concise and factual.

## Final output format

```text
Memory files updated:
Rationale:
Project memory result: PASS or BLOCKED
```
