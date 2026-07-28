---
name: push-readiness
description: Determines whether a committed dedicated branch is safe and explicitly authorized to push by consuming commit-readiness, verification, and secret-scan evidence and checking branch, upstream, worktree, outgoing scope, and accepted approval phrases. Use only before a requested push. Readiness never grants approval and must not push, create a PR, or modify repository state.
---

# Push Readiness

## Required input

Obtain the current branch, upstream and remote, commits to push, final worktree status, the user's exact push approval phrase, and completed evidence containing `COMMIT_READINESS: PASS`, `VERIFICATION_STATUS: PASS`, and `SECRET_SCAN_STATUS: PASS`.

## Workflow

1. Read push gates in `instructions/01-execution-and-planning.md` and `instructions/05-completion.md`.
2. Consume the existing commit-readiness, executed-verification, and secret-scan results. Do not rerun or imitate those Skills.
3. Require `COMMIT_READINESS: PASS`, `VERIFICATION_STATUS: PASS`, and `SECRET_SCAN_STATUS: PASS`; missing, stale, contradictory, failed, or blocked required evidence makes push readiness `BLOCKED`.
4. Confirm the branch is dedicated, is not `main`, and has a clear intended remote and upstream.
5. Confirm the worktree is clean and inspect all outgoing commits for scope and unrelated history.
6. Confirm no unresolved blocker remains in the consumed evidence or outgoing scope.
7. Accept only repository-defined explicit push approval, including `APPROVED: PUSH` or `APPROVED: PUSH BACKEND BRANCH`.
8. Report readiness without pushing.

## Output contract

```text
Branch:
Upstream:
Outgoing commits:
Worktree:
Approval:
Commit readiness evidence:
Verification evidence:
Secret scan evidence:
Unresolved blockers:
PUSH_READINESS: PASS | BLOCKED
```

## Validation scenarios

- Positive: scoped commits on a clean feature branch with explicit accepted push approval and all three required `PASS` evidence results -> `PASS`.
- Negative: commit-readiness or implementation request -> do not trigger.
- Valid output: one readiness result and no external action.
- Blocked: missing or non-passing required evidence, unresolved blocker, `main`, dirty worktree, unclear remote, unrelated commits, or missing approval -> `BLOCKED`.
