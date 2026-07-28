---
name: verification-runner
description: Plans or executes proportionate repository verification for code, project, configuration, migration, CI, runtime-behavior documentation, or documentation-only changes. Dry runs classify checks without claiming success; executed runs report exact results. Use after implementation and before completion or commit readiness, or when a verification dry run is requested. Do not conceal failures or run destructive database operations without approval.
---

# Verification Runner

## Required input

Obtain the mode (`dry-run` or `execute`), approved scope, change type, changed files, and relevant repository verification guidance.

## Workflow

1. Read verification defaults in `docs/project/PROMPT_TEMPLATE.md`, current commands and cautions in `docs/project/AI_HANDOFF.md`, and completion rules in `instructions/05-completion.md`.
2. Load scope-specific guidance only when the changed surface requires it: architecture guidance for boundary changes, database guidance for database-impacting work, security guidance for security-sensitive work, and API guidance for behavior-visible API changes.
3. Classify restore, build, unit tests, integration tests, architecture tests, migration checks, manual API checks, security checks, frontend checks, documentation self-review, and diff validation as `Required`, `Not required`, `Deferred`, or `Blocked`, with a reason.
4. In `dry-run` mode, do not execute checks and do not emit `VERIFICATION_STATUS: PASS`. Return only the classification plan and dry-run terminal result.
5. In `execute` mode, run every required check that is authorized and available. For documentation-only maintenance, normally run documentation self-review and diff checks and justify why code checks are not required.
6. Record every command, exit result, meaningful warning, and skipped check with rationale.
7. Return `FAIL` when an executed required check fails. Return `BLOCKED` when required verification cannot run, authorization is missing, evidence is incomplete, or a required check is unjustifiably omitted. Never return `PASS` for checks that were not executed.

## Dry-run output contract

```text
Mode: dry-run
Verification plan:
- Restore: Required | Not required | Deferred | Blocked — reason
- Build: Required | Not required | Deferred | Blocked — reason
- Unit tests: Required | Not required | Deferred | Blocked — reason
- Integration tests: Required | Not required | Deferred | Blocked — reason
- Architecture tests: Required | Not required | Deferred | Blocked — reason
- Migration checks: Required | Not required | Deferred | Blocked — reason
- Manual API checks: Required | Not required | Deferred | Blocked — reason
- Security checks: Required | Not required | Deferred | Blocked — reason
- Frontend checks: Required | Not required | Deferred | Blocked — reason
- Documentation self-review: Required | Not required | Deferred | Blocked — reason
- Diff validation: Required | Not required | Deferred | Blocked — reason
VERIFICATION_DRY_RUN: COMPLETE | BLOCKED
```

## Executed output contract

```text
Change type:
Commands:
Results:
Warnings:
Skipped:
VERIFICATION_STATUS: PASS | FAIL | BLOCKED
```

## Validation scenarios

- Positive: completed code change in execute mode -> run solution and targeted checks and return the actual result.
- Negative: dry-run request -> classify all check categories without executing or claiming `PASS`.
- Valid output: documentation-only change may `PASS` with diff/self-review and a clear build/test skip rationale.
- Failed: an executed required command returns a failing result -> `FAIL`.
- Blocked: tooling, authorization, or required context prevents a required check from running, or required verification is omitted -> `BLOCKED`.
