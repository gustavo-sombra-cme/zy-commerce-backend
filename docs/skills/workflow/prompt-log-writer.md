# prompt-log-writer

## Purpose

Create chronological prompt logs for planning, execution, testing, documentation, and review work.

## When to use

Use before execution unless the user explicitly writes `SKIP PROMPT LOG`.

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

## Final output format

```text
Prompt log:
Status:
Result: PASS or BLOCKED
```
