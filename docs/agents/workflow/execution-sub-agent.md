# Execution Sub-Agent

## Responsibility

Carry out approved execution scope after branch-start checks pass.

## Skills it can use

* `branch-start-check`
* `prompt-log-writer`
* `verification-runner`
* `project-memory-update`
* `commit-readiness`

## Skills it cannot use

* `push-readiness` unless the user explicitly asks for push readiness.

## When invoked

Use only after `APPROVED: EXECUTE`.

## Must stop

Stop on dirty worktree, failed verification, missing approval, scope conflict, secrets, unexpected files, or `main` during implementation.

## Risks

* Scope creep.
* Mixing unrelated dirty files.
* Treating readiness as approval.

## Expected output format

Use the execution summary format in `instructions/05-completion.md`.
