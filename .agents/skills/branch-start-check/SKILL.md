---
name: branch-start-check
description: Verifies that an explicitly approved repository execution starts from the correct path, an up-to-date main branch, a clean worktree, and a dedicated task branch. Use before implementation for APPROVED: EXECUTE tasks. Do not use to bypass dirty-worktree safety or to authorize execution, commits, pushes, or destructive Git operations.
---

# Branch Start Check

## Required input

Obtain the approved task, repository path, proposed branch name, and any explicit approval covering existing dirty changes or a separate worktree.

## Workflow

1. Read `instructions/01-execution-and-planning.md` and the execution rules in `docs/project/PROMPT_TEMPLATE.md`.
2. Confirm the repository path, current branch, remotes, and `git status --short --branch`.
3. Stop on a dirty worktree unless the user explicitly approved including those changes or using a separate clean worktree. Never stash, reset, overwrite, or switch a dirty tree implicitly.
4. Confirm `main` freshness using the repository-approved fetch/pull flow.
5. Create or confirm one dedicated `feature/`, `fix/`, `docs/`, or `chore/` branch only within the approved execution scope.
6. Report the evidence before implementation.

## Output contract

```text
Repository:
Starting branch:
Worktree:
Main freshness:
Task branch:
BRANCH_START: PASS | BLOCKED
```

## Validation scenarios

- Positive: approved task, clean current main, clear remote, valid new branch -> `PASS`.
- Negative: planning-only or status-only request -> do not trigger.
- Valid output: contains all contract fields and one result.
- Blocked: dirty worktree without explicit handling approval, stale main, unclear remote, or invalid branch name -> `BLOCKED`.
