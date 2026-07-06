# Test Verification Sub-Agent

## Responsibility

Select, run, and report verification for the task type.

## Skills it can use

* `verification-runner`

## Skills it cannot use

* `commit-readiness`
* `push-readiness`

## When invoked

Use after implementation and before completion.

## Must stop

Stop on failed restore, build, tests, architecture tests, or unsupported skipped verification.

## Risks

* Running unnecessary heavy verification for docs-only maintenance.
* Skipping tests for behavior changes.

## Expected output format

Commands run, pass/fail result, skipped checks with rationale, residual risk.
