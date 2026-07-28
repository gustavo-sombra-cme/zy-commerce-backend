# Execution And Planning

## STATE MACHINE

The agent must operate in the following states:

1. INTENT_ANALYSIS
2. ARCHITECTURE_REVIEW
3. PLANNING
4. EXECUTION
5. SELF_REVIEW
6. TESTING
7. COMPLETION

---

# EXECUTION LOCK

Execution is forbidden unless the user explicitly approves execution.

Approval phrase:

APPROVED: EXECUTE

Without approval:

* Do not generate code
* Do not create files
* Do not modify files
* Do not run commands
* Do not scaffold projects

---

# PLANNING RULES

Planning prompts use the Planning Sub-Agent behavior in `docs/agents/workflow/planning-sub-agent.md`.

Apply `.agents/skills/prompt-log-writer/SKILL.md` before repository planning unless the user explicitly writes `SKIP PROMPT LOG`. The prompt log is the sole governance artifact permitted by a planning request; it records the plan and does not authorize implementation or any other repository modification.

Use `.agents/skills/architecture-decision-check/SKILL.md` when planning boundary, module, runtime AI autonomy, Text-to-SQL strategy, database, package, API, migration, or cross-module changes.

Planning responses must contain, at minimum:

1. Architecture Overview
2. Design Overview
3. Dependency Impact
4. Files Affected
5. Testing Strategy
6. Risks
7. Execution Checklist

Every plan must end with:

PLAN_STATUS: PENDING_APPROVAL

For short planning prompts such as "Plan next Catalog feature: Update Product Details", the expanded response must follow the full Plan Output Contract in `docs/project/PROMPT_TEMPLATE.md`.

---

# SHORT PROMPT RULES

Short planning prompts such as "Plan next Catalog feature: Update Product Details" must be expanded using:

* `AGENT.md`
* `instructions/*`
* `docs/project/PROMPT_TEMPLATE.md`
* current project memory under `docs/project/`
* relevant recent prompt logs and ADRs

Short planning prompts must return every required section from `docs/project/PROMPT_TEMPLATE.md#required-plan-output` using the exact section names and must perform the template's Plan Self-Validation Rule before responding.

Short prompts must use applicable repository Skills under `.agents/skills/` for repeated workflow checks instead of duplicating their instructions in every response.

Short planning prompts must not invoke execution, commit, push, PR, migration execution, or destructive workflow skills.

Short execution prompts such as "Execute approved feature: Update Product Details" are not valid execution approval unless they include the explicit approval phrase:

APPROVED: EXECUTE

Except for the prompt log required by the repository prompt-logging rule, the execution lock remains mandatory for all file creation, file modification, code generation, command execution, scaffolding, migrations, and project changes.

Short prompts do not override architecture, DDD, CQRS, module isolation, documentation, prompt logging, testing, security, or completion rules.

---

# BRANCH WORKFLOW RULES

* One task = one branch = one PR.
* Every approved execution task must start from latest `main` unless the user explicitly says otherwise.
* Create a new dedicated branch before changing files.
* Do not continue new feature work on an old feature branch.
* Do not work directly on `main`.
* Do not push directly to `main`.
* Use branch names in one of these forms:
  * `feature/<feature-name>`
  * `fix/<bug-name>`
  * `docs/<documentation-change>`
  * `chore/<maintenance-task>`

## START-OF-EXECUTION FLOW

For every `APPROVED: EXECUTE` task:

Apply Execution Sub-Agent behavior from `docs/agents/workflow/execution-sub-agent.md`.

Apply `.agents/skills/branch-start-check/SKILL.md` before implementation.

Apply `.agents/skills/prompt-log-writer/SKILL.md` before implementation unless the user explicitly writes `SKIP PROMPT LOG`.

1. Confirm the current repository path.
2. Confirm the current branch.
3. Run `git status --short --branch`.
4. If the worktree has uncommitted changes, stop and report unless the user explicitly approves a separate worktree or explicitly approves including those changes.
5. Fetch latest `main`.
6. Check out `main`.
7. Pull latest `main`.
8. Create a new dedicated branch for the approved task.
9. Confirm the branch name before making changes.
10. Implement only after the new branch exists.

## DIRTY WORKTREE SAFETY

* Do not switch branches, reset, stash, or overwrite files in a dirty worktree without explicit approval.
* If a new task must start while the primary worktree is dirty, propose a separate clean Git worktree.
* Do not include unrelated dirty files in a task branch.
* Dirty worktree safety applies before branch switches, file edits, prompt-log edits, commits, pushes, or verification that may create artifacts.

## MANUAL COMMIT AND PUSH GATE

* Do not commit automatically unless explicitly approved.
* Do not push automatically unless explicitly approved.
* Do not create a pull request automatically unless explicitly approved.
* After implementation and verification, stop for local review and wait for one of:
  * `APPROVED: COMMIT BACKEND CHANGES`
  * `APPROVED: PUSH`
  * `APPROVED: PUSH BACKEND BRANCH`
  * `APPROVED: CREATE BACKEND PR`
  * `APPROVED: COMMIT AND PUSH BACKEND CHANGES`

## PRE-COMMIT CHECKS

Before any approved commit, run or confirm:

* `git status --short --branch`
* `git diff --name-only`
* Changed files were reviewed.
* Applicable repository Skills under `.agents/skills/` were applied.
* No unrelated files are included.
* No secrets are included.
* No generated artifacts are included.
* No `bin`, `obj`, `TestResults`, or `coverage` files are included.
* Required build/test verification passed when application files changed.

`APPROVED: PUSH` and `APPROVED: PUSH BACKEND BRANCH` are both valid explicit backend push approval phrases. A readiness skill can recommend a push, but it is never approval by itself.

Stop and report if build fails, tests fail, secrets are detected, unexpected files changed, the current branch is `main` during implementation, `main` is not up to date, the remote is unclear, or the worktree is dirty without approval for the task.
