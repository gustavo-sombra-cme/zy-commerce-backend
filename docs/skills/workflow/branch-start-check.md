# branch-start-check

## Purpose

Verify that an approved execution task starts from the correct repository, latest `main`, a clean worktree, and a dedicated task branch.

## When to use

Required before implementation for every `APPROVED: EXECUTE` task.

## Inputs

* Repository path.
* Approved task name.
* Proposed branch name.
* Whether the user approved a separate worktree or including dirty changes.

## Outputs

* Repository path confirmation.
* Current branch confirmation.
* `git status --short --branch` result.
* Main freshness result.
* Created or confirmed task branch.

## Required reading

* `instructions/01-execution-and-planning.md`
* `docs/project/PROMPT_TEMPLATE.md`

## Files it may read

* Git metadata.
* `AGENT.md`
* `instructions/*`

## Files it may update

None directly. Git branch metadata may change only after explicit execution approval.

## Stop conditions

* Worktree is dirty without explicit approval.
* Any attempt is made to stash, reset, switch branches, or overwrite a dirty tree without explicit approval.
* Current branch is not `main` when the task requires starting from `main`.
* `main` is not up to date.
* Remote is unclear.
* Proposed branch name does not use `feature/`, `fix/`, `docs/`, or `chore/`.

## Verification expectations

Confirm the active branch before implementation begins.

## Final output format

```text
Repository:
Starting branch:
Status:
Main freshness:
Task branch:
Result: PASS or BLOCKED
```

## Example output

```text
Repository: C:\ZippyYum\Learning\zy-commerce-backend
Starting branch: main
Status: ## main...origin/main
Main freshness: latest origin/main fetched and fast-forwarded
Task branch: docs/example-task
Result: PASS
```
