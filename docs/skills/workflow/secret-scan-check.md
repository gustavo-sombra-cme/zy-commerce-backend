# secret-scan-check

## Purpose

Prevent secrets and sensitive configuration from entering commits.

## When to use

Use before every commit and before push readiness.

## Inputs

* Changed files.
* Diff.

## Outputs

* Secret scan result.
* Suspicious files or values.

## Required reading

* `instructions/03-cqrs-database-testing-security.md`
* `docs/project/AI_HANDOFF.md`

## Files it may read

* Changed config files.
* Diff.
* Documentation containing setup examples.

## Files it may update

None.

## Stop conditions

* API key, token, password, real connection string, bearer token, or private credential appears in diff.
* Documentation example uses real-looking secret values instead of placeholders.

## Verification expectations

Confirm `appsettings*.json` does not receive real secrets.

## Final output format

```text
Files checked:
Findings:
Secret scan: PASS or BLOCKED
```
