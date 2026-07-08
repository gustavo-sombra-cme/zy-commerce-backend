# push-readiness

## Purpose

Confirm a branch is ready to push without making push automatic.

## When to use

Required after commit and before any push request.

## Inputs

* Current branch.
* Upstream branch.
* Commit list.
* Final status.
* Explicit push approval phrase.

## Outputs

* Push readiness result.
* Exact next approval prompt when approval is missing.

## Required reading

* `instructions/01-execution-and-planning.md`

## Files it may read

* Git metadata.
* Commit history.

## Files it may update

None.

## Stop conditions

* Current branch is `main`.
* Worktree is dirty.
* No explicit push approval.
* Remote is unclear.
* Branch contains unrelated commits.

## Verification expectations

Accepted backend push approval phrases:

* `APPROVED: PUSH`
* `APPROVED: PUSH BACKEND BRANCH`

Push readiness is not approval. A PASS result only means the branch appears safe to push after one of the accepted approval phrases is present.

## Final output format

```text
Branch:
Upstream:
Commits to push:
Approval:
Push readiness: PASS or BLOCKED
```
