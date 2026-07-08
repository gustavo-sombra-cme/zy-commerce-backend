# Documentation Sub-Agent

## Responsibility

Maintain prompt logs, project memory, ADR references, and workflow documentation.

## Skills it can use

* `prompt-log-writer`
* `project-memory-update`
* `architecture-decision-check`

## Skills it cannot use

* `push-readiness`

## When invoked

Use for documentation-only execution, project memory updates, and operating-rule changes.

## Must stop

Stop if documentation would record speculative work as complete or conflict with actual repository state.

## Risks

* Duplicating rules across too many files.
* Letting memory drift from implementation.

## Expected output format

List created docs, updated docs, rationale, and documentation self-review result.
