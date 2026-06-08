# Prompt 028 - Agent Split Execution

## Date

2026-06-08

## Purpose

Execute the approved AGENT.md split plan.

## Full Prompt

APPROVED: EXECUTE

Execute the AGENT.md split plan exactly.

Before execution:
- create/update docs/prompts/027-agent-split-planning.md
- create docs/prompts/028-agent-split-execution.md

Create:
- instructions/00-role-and-stack.md
- instructions/01-execution-and-planning.md
- instructions/02-architecture-and-modules.md
- instructions/03-cqrs-database-testing-security.md
- instructions/04-documentation-and-memory.md
- instructions/05-completion.md

Update:
- AGENT.md by full replacement into a short router
- docs/project/PROJECT_STATUS.md
- docs/project/AI_HANDOFF.md
- docs/project/ROADMAP.md

Rules:
- Preserve all existing V2 rules exactly.
- Use stable file/section references, not line numbers.
- Do not modify source code, tests, migrations, projects, modules, or package references.

Report:
- files changed
- AGENT.md full replacement status
- instruction files created
- rule preservation status
- memory docs updated
- deviations

## Status

EXECUTED

## Result Summary

Split AGENT.md into a short router and instruction files while preserving the existing V2 rule sections.
