# architecture-decision-check

## Purpose

Determine whether a proposed change requires an ADR or explicit architecture approval.

## When to use

Required for new modules, packages, APIs, runtime AI autonomy, Text-to-SQL strategy changes, migrations, cross-module integration, or major boundary changes.

## Inputs

* Proposed change.
* Affected files.
* Existing ADRs.

## Outputs

* ADR required yes/no.
* Rationale.
* Existing ADRs affected.

## Required reading

* `instructions/*`
* `docs/decisions/*`
* `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md`

## Files it may read

* ADRs.
* Project memory.
* Architecture tests.

## Files it may update

* ADR files only when explicitly approved.

## Stop conditions

* Major architecture decision lacks ADR.
* Existing ADR would be contradicted.
* Requested change violates project restrictions without explicit approval.

## Verification expectations

State why an ADR is or is not required.

ADR example: introducing runtime assistant sub-agent classes that change API orchestration boundaries.

Non-ADR example: adding examples to repo-local workflow skill docs without changing runtime or architecture.

## Final output format

```text
ADR required:
Affected ADRs:
Decision:
Architecture check: PASS or BLOCKED
```
