---
name: commit-readiness
description: Determines whether an approved task branch is ready for a local commit by composing branch safety, scope, code-review, executed-verification, secret-scan, applicable migration-safety, prompt-log, and project-memory evidence. Use immediately before an explicitly approved local commit. Readiness is not commit permission; this Skill must not rerun checks, commit, push, or modify files.
---

# Commit Readiness

## Required input

Obtain the approved scope, current branch and status, changed-file list, current-task `BRANCH_START: PASS`, and completed evidence for code review, executed verification, secret scan, migration safety when applicable, prompt-log completion when required, and project-memory update when verified persistent state changed.

## Workflow

1. Read the commit gates in `instructions/01-execution-and-planning.md` and `instructions/05-completion.md`.
2. Consume existing evidence; do not rerun or imitate the producing Skills.
3. Require current-task `BRANCH_START: PASS`, confirm the branch is dedicated and is not `main`, and use read-only Git state only to confirm the current branch and changed-file set still match the consumed evidence. Evidence is stale when it predates the latest scoped file change, names a different branch, or covers a different changed-file set; if freshness cannot be established, return `BLOCKED`.
4. Compare every changed file with the approved scope and record `Scope: PASS` or `Scope: BLOCKED`. Reject unrelated files and generated output.
5. Require `VERIFICATION_STATUS: PASS` from executed verification and `SECRET_SCAN_STATUS: PASS`.
6. Require `CODE_REVIEW: PASS` when code review applies; otherwise record `Code review: NOT_REQUIRED — rationale`.
7. Require `MIGRATION_SAFETY: PASS` when migration safety applies; otherwise record `NOT_APPLICABLE`.
8. Require `PROMPT_LOG: PASS` when prompt logging applies; otherwise record `Prompt log: NOT_REQUIRED — rationale`.
9. When verified work changed persistent project state, require `PROJECT_MEMORY_UPDATE: PASS`; otherwise record `Project memory: NOT_REQUIRED — rationale`.
10. Treat missing, stale, contradictory, failed, or blocked required evidence, scope drift, or unresolved blockers as `BLOCKED`.
11. Check for `bin`, `obj`, `TestResults`, `coverage`, packaged archives, and other generated artifacts.
12. Report readiness without performing a commit.

## Output contract

```text
Branch:
Branch-start evidence:
Changed files:
Scope: PASS | BLOCKED
Verification:
Code review:
Secret scan:
Migration safety:
Prompt log:
Project memory:
Unresolved blockers:
Generated artifacts:
COMMIT_READINESS: PASS | BLOCKED
```

## Validation scenarios

- Positive: dedicated branch with scoped changes and all applicable evidence present and passing -> `PASS`.
- Negative: ordinary implementation or planning request -> do not trigger until commit readiness is requested.
- Valid output: includes one readiness result and no commit action.
- Blocked: missing required evidence, `main`, unsafe branch state, unrelated files, failed or blocked checks, secrets, unresolved blockers, or generated artifacts -> `BLOCKED`.
