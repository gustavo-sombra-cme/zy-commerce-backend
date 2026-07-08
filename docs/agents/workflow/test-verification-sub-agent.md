# Test Verification Sub-Agent

## Responsibility

Select, run, and report verification for the task type.

## Allowed skills

* `verification-runner`

## Forbidden actions

* `commit-readiness`
* `push-readiness`
* Skipping required tests for behavior changes without rationale.
* Running migrations or live provider calls unless explicitly approved.

## When invoked

Use after implementation and before completion.

## Stop conditions

Stop on failed restore, build, tests, architecture tests, or unsupported skipped verification.

## Risks

* Running unnecessary heavy verification for docs-only maintenance.
* Skipping tests for behavior changes.

## Expected output format

Commands run, pass/fail result, skipped checks with rationale, residual risk.
