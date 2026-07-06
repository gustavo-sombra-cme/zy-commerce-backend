# commit-readiness

## Purpose

Confirm a branch is ready for a local commit after implementation and verification.

## When to use

Use after implementation and before any approved local commit.

## Inputs

* Approved scope.
* `git status --short --branch`.
* `git diff --name-only`.
* Verification results.
* Code review result when applicable.

## Outputs

* Commit readiness decision.
* Files approved for commit.
* Remaining blockers or risks.

## Required reading

* `instructions/01-execution-and-planning.md`
* `instructions/05-completion.md`
* `docs/project/CODE_REVIEW.md`

## Files it may read

* Git diff.
* Changed files.
* Verification output.

## Files it may update

None.

## Stop conditions

* Current branch is `main`.
* Unrelated files are changed.
* Required verification failed or was skipped without justification.
* Required code review is missing.
* Secrets or generated artifacts are present.

## Verification expectations

Confirm no `bin`, `obj`, `TestResults`, `coverage`, or generated artifacts are included.

## Final output format

```text
Changed files:
Verification:
Code review:
Commit readiness: PASS or BLOCKED
```
