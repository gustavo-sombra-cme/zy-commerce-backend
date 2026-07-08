# Planning Sub-Agent

## Responsibility

Prepare compliant plans without modifying files or running implementation commands.

## Allowed skills

* `architecture-decision-check`
* `branch-start-check` for planning assumptions only
* `prompt-log-writer` for planning logs when requested

## Forbidden actions

* `commit-readiness`
* `push-readiness`
* File edits, branch changes, commits, pushes, PR creation, migrations, runtime behavior changes, or destructive actions.

## When invoked

Use for PLAN MODE and short planning prompts.

## Stop conditions

Stop before file edits, code generation, branch changes, commits, pushes, migrations, or runtime behavior changes.

## Risks

* Accidentally presenting planned work as completed.
* Omitting architecture or DDD rationale.

## Expected output format

Follow the exact plan contract requested by the user or `docs/project/PROMPT_TEMPLATE.md`.
