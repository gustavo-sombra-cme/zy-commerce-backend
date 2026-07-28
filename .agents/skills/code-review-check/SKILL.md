---
name: code-review-check
description: Reviews a repository diff for defects, regressions, architecture violations, missing tests, security problems, and scope drift. Use before committing code, project or configuration files, migrations, CI workflows, or runtime-behavior documentation, and when a user requests a findings-first review. Do not use as commit approval or for simple documentation proofreading unless requested.
---

# Code Review Check

## Required input

Obtain the approved scope, changed-file list, diff, and available verification evidence.

## Workflow

1. Read `docs/project/CODE_REVIEW.md` and only the repository instructions relevant to the changed surface.
2. When the user explicitly requests architecture compliance, or the diff can affect dependency direction, bounded contexts, module ownership, public contracts, security boundaries, persistence ownership, external integrations, deployment, runtime AI autonomy, agent orchestration, or technology choices, load `instructions/02-architecture-and-modules.md`.
3. Load specific ADRs only when the diff changes architecture, the user explicitly requests ADR compliance, or repository evidence identifies a possibly governing ADR. Do not load every ADR by default.
4. Load database, migration, raw SQL, Text-to-SQL, or connection-target guidance only when the diff actually changes that surface. An unrelated configuration flag does not make database guidance applicable.
5. Inspect the complete diff and enough surrounding code or documentation to verify behavior.
6. Prioritize concrete defects, security exposure, architecture or module-boundary violations, contract drift, missing coverage, and unrelated changes.
7. Label findings `BLOCKER`, `HIGH`, `MEDIUM`, or `LOW`; include file and line evidence when possible.
8. State whether build/test evidence is present or why it is not required. Do not edit files unless the user separately requests fixes.

## Output contract

```text
Findings:
Open questions:
Verification evidence:
CODE_REVIEW: PASS | BLOCKED
Commit blocked: YES | NO
Required follow-up:
```

## Validation scenarios

- Positive: review an implementation diff before commit -> trigger and report findings first.
- Negative: implement a requested feature -> review only after implementation, not instead of it.
- Valid output: no findings means explicit `PASS`; any blocker means `BLOCKED`.
- Loading: explicit architecture-compliance review loads `instructions/02-architecture-and-modules.md`; unrelated database guidance remains unloaded unless the diff changes that surface.
- Blocked: incomplete diff, missing required context, secret exposure, regression, or boundary violation -> `BLOCKED`.
