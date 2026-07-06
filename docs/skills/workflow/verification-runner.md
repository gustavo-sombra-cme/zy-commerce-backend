# verification-runner

## Purpose

Select and run the appropriate verification for the change type.

## When to use

Use after implementation and before commit readiness.

## Inputs

* Change type.
* Changed files.
* Approved scope.

## Outputs

* Verification commands run.
* Results.
* Skipped checks with rationale.

## Required reading

* `docs/project/PROMPT_TEMPLATE.md`
* `docs/project/AI_HANDOFF.md`

## Files it may read

* Solution and project files.
* Changed files.
* Test projects.

## Files it may update

No source files. Build/test tools may create local ignored outputs.

## Stop conditions

* Restore, build, or test fails.
* Required verification is skipped without rationale.
* Runtime behavior changed without tests.

## Verification expectations

For code or project changes, normally run:

```text
dotnet restore Ecommerce.sln
dotnet build Ecommerce.sln
dotnet test Ecommerce.sln
```

For documentation-only maintenance, perform documentation self-review.

## Final output format

```text
Change type:
Commands:
Results:
Skipped:
Verification: PASS or BLOCKED
```
