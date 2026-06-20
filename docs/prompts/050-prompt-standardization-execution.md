# Prompt 050 - Prompt Standardization Execution

## Prompt Number

050

## Date

2026-06-09

## Purpose

Execute the approved documentation-only prompt standardization and reusable template setup.

## Full Prompt

APPROVED: EXECUTE Prompt Standardization And Reusable Template Setup

Execute the approved documentation-only plan.

Task:
Review existing prompt logs and standardize recurring prompt workflow guidance so future prompts can be shorter.

Approved scope:

Create docs/project/PROMPT_TEMPLATE.md
Add a short reference to the reusable template in AGENT.md
Add concise short-prompt/default-mode language to instructions/01-execution-and-planning.md
Add DDD ownership clarification to instructions/03-cqrs-database-testing-security.md
Add template-location clarification to instructions/04-documentation-and-memory.md
Add default execution-summary format to instructions/05-completion.md
Update project memory docs:
docs/project/PROJECT_STATUS.md
docs/project/AI_HANDOFF.md
docs/project/ROADMAP.md
docs/project/NEXT_SESSION.md
Create prompt logs under docs/prompts/ for this planning/execution work

Rules:

Documentation-only task
Do not change application code
Do not change project files
Do not create migrations
Do not add packages
Do not weaken execution lock rules
Do not weaken prompt logging rules
Do not weaken Clean Architecture, DDD, CQRS, module isolation, or testing rules
Keep AGENT.md as a router
If AGENT.md is changed, replace the full file, not a partial edit
Do not rewrite old prompt logs; they are historical records

Verification:

Do not run dotnet restore, dotnet build, or dotnet test unless code or project structure changes are accidentally required
If only documentation changes are made, perform documentation self-review only

Return:

Summary of changes
Files changed
Final location of the reusable prompt template
How to use shorter prompts going forward
Verification performed
Risks or follow-ups
TASK_STATUS

## Status

EXECUTED

## Result Summary

Created `docs/project/PROMPT_TEMPLATE.md`, updated `AGENT.md` by full replacement with a short template reference, added concise short-prompt/default-mode guidance to instruction files, updated project memory documentation, and performed documentation self-review. No application code, project files, migrations, or packages were changed.
