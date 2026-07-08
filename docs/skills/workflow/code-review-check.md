# code-review-check

## Purpose

Review changes for bugs, architecture risks, missing tests, security issues, and scope drift.

## When to use

Required before commits that include code, project/configuration files, migrations, CI workflow files, or runtime-behavior documentation changes. Optional for documentation-only maintenance.

## Inputs

* Approved scope.
* Changed files.
* Diff.
* Verification results.

## Outputs

* Findings-first review.
* Open questions.
* PASS or BLOCKED result.

Severity labels:

* `BLOCKER` blocks commit.
* `HIGH` normally blocks commit unless explicitly accepted.
* `MEDIUM` requires follow-up or clear rationale.
* `LOW` is informational or cleanup-level.

## Required reading

* `docs/project/CODE_REVIEW.md`
* `instructions/*`
* Relevant ADRs.

## Files it may read

* Changed files.
* Tests.
* `docs/decisions/*`
* `docs/project/*`

## Files it may update

None.

## Stop conditions

* Bug or behavior regression.
* Architecture or module-boundary violation.
* Missing tests for behavior changes.
* Secret or sensitive data exposure.
* Unrelated files included.

## Verification expectations

Review must mention whether build/test evidence is present or why it is not required.

## Final output format

```text
Findings:
Open Questions:
Review Result: PASS or BLOCKED
Commit Blocked: YES or NO
Required Follow-up:
```
