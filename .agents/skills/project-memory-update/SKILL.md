---
name: project-memory-update
description: Updates only the repository memory files affected by a verified persistent state change, including a feature, bug fix, ADR, migration, runtime configuration, Skill workflow behavior, source-of-truth consolidation, tests, risks, or roadmap state. Do not trigger for plans, hypothetical or unverified work, grammar-only edits, or temporary test artifacts.
---

# Project Memory Update

## Required input

Obtain the completed-task summary, scoped changed files, verification results, and any new constraints or follow-up work.

## Workflow

1. Read the project-memory rule in `instructions/04-documentation-and-memory.md`.
2. Confirm a verified persistent state change exists. Trigger for implemented features or bug fixes, accepted ADRs, migrations, runtime configuration, repository Skill/workflow behavior, source-of-truth consolidation, durable test status, known risks, or roadmap state.
3. Do not trigger for a plan, hypothetical or incomplete work, unverified claims, grammar-only edits, or temporary test artifacts.
4. Select memory files by purpose and load only those needed:
   - `PROJECT_STATUS.md` for what currently exists and verified build/test state.
   - `AI_HANDOFF.md` for constraints, gotchas, operating guidance, and recent state the next session must know.
   - `ROADMAP.md` for completed, current-priority, candidate, or explicitly not-started work.
   - `NEXT_SESSION.md` for fast-resume state, last completed work, next approved task, commands, and warnings.
5. Compare verified repository state with each selected document and update only affected facts. An unchanged file must have a purpose-based rationale and need not be loaded merely to say it is unchanged.
6. Preserve historical prompt logs and avoid copying execution history into reusable Skills.
7. Reject claims not supported by the diff or verification evidence.

## Output contract

```text
Memory files updated:
Facts recorded:
Files unchanged and rationale:
PROJECT_MEMORY_UPDATE: PASS | BLOCKED
```

## Validation scenarios

- Positive: completed feature, bug fix, or verified workflow-rule change persists repository state -> update only applicable memory and return `PASS`.
- Negative: plan, hypothetical work, grammar-only correction, or temporary test artifact -> do not trigger.
- Valid output: names each updated or intentionally unchanged memory file.
- Blocked: implementation or verification is incomplete, or sources conflict -> `BLOCKED`.
