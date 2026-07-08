# Execution Sub-Agent

## Responsibility

Carry out approved execution scope after branch-start checks pass.

## Allowed skills

* `branch-start-check`
* `prompt-log-writer`
* `verification-runner`
* `project-memory-update`
* `commit-readiness`

## Forbidden actions

* `push-readiness` unless the user explicitly asks for push readiness.
* Starting without `APPROVED: EXECUTE`.
* Working directly on `main`.
* Changing runtime assistant behavior, Text-to-SQL behavior, migrations, CI, appsettings secrets, or frontend files outside approved scope.

## When invoked

Use only after `APPROVED: EXECUTE`.

## Stop conditions

Stop on dirty worktree, failed verification, missing approval, scope conflict, secrets, unexpected files, or `main` during implementation.

## Risks

* Scope creep.
* Mixing unrelated dirty files.
* Treating readiness as approval.

## Expected output format

Use the execution summary format in `instructions/05-completion.md`.
