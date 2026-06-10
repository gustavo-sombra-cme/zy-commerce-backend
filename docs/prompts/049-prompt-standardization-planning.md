# Prompt 049 - Prompt Standardization Planning

## Prompt Number

049

## Date

2026-06-09

## Purpose

Plan standardizing recurring prompt workflow guidance so future planning and execution prompts can be shorter.

## Full Prompt

PLAN MODE

Task:
Review all existing prompt logs under `docs/prompts/` and extract the repeated/common rules, patterns, constraints, and workflow expectations.

Goal:
Reduce repetitive future chat prompts by moving stable recurring instructions into project documentation.

Source files to inspect:

* `AGENT.md`
* `instructions/*`
* `docs/prompts/*`
* `docs/project/PROJECT_STATUS.md`
* `docs/project/AI_HANDOFF.md`
* `docs/project/ROADMAP.md`
* `docs/project/NEXT_SESSION.md`

Do not execute code.
Do not modify files yet.
Do not create code.
This is planning only.

Analyze:

1. Common sections repeated across planning prompts
2. Common sections repeated across execution prompts
3. Common architecture constraints
4. Common DDD / Clean Architecture / CQRS rules
5. Common documentation update rules
6. Common verification rules
7. Common approval and execution-lock rules
8. Any conflicts or duplication between AGENT.md, instructions/*, and prompt logs

Propose changes:

1. What should be added to `AGENT.md`
2. What should remain in `instructions/*`
3. What should not be duplicated
4. Whether `AGENT.md` should stay a router or include more common prompt rules
5. A new reusable prompt template file, for example:

   * `docs/prompts/PROMPT_TEMPLATE.md`
   or
   * `docs/project/PROMPT_TEMPLATE.md`

The template should let me write shorter chat prompts like:
"Plan next Catalog feature: Update Product Details"
or:
"Execute approved feature: Update Product Details"

The template must include default expectations for:

* PLAN MODE
* APPROVED EXECUTE mode
* architecture rules
* DDD ownership
* CQRS expectations
* file/documentation updates
* test verification
* output format
* risks and follow-ups

Return:

1. Summary of repeated prompt patterns found
2. Recommended AGENT.md changes
3. Recommended instruction-file changes, if any
4. Proposed prompt template file content
5. Migration plan
6. Risks
7. Execution checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned a documentation-only prompt standardization update: keep `AGENT.md` as a router, create `docs/project/PROMPT_TEMPLATE.md`, add concise short-prompt/default-mode guidance to instruction files, and update project memory without changing application code.
