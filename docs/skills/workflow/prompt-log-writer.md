# prompt-log-writer

## Purpose

Create chronological prompt logs for planning, execution, testing, documentation, and review work.

## When to use

Required before execution unless the user explicitly writes `SKIP PROMPT LOG`.

## Inputs

* Prompt text.
* Date.
* Purpose.
* Status.
* Result summary when available.

## Outputs

* New prompt log file under `docs/prompts/`.

## Required reading

* `instructions/04-documentation-and-memory.md`

## Files it may read

* Existing files in `docs/prompts/`.

## Files it may update

* `docs/prompts/NNN-title.md`

## Stop conditions

* Duplicate prompt number.
* Missing full prompt text.
* Attempt to rewrite historical prompt logs for style cleanup.

## Verification expectations

Confirm the prompt number is the next available number.

If `090-title.md` already exists, the next log must use `091-...md`; do not reuse `090` with a different title.

Prompt status lifecycle is:

* `PLANNED` for planning records.
* `APPROVED` when execution is approved but not complete.
* `EXECUTED` after successful completion.
* `FAILED` when execution stops because of a blocker or failed verification.

## Final output format

```text
Prompt log:
Status:
Result: PASS or BLOCKED
```
