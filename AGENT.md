# E-Commerce Backend Agent Instructions

This file is the entry point for agent behavior.

Always read this file first.

Do not read every instruction file by default. Load only the files needed for the current task.

Use `instructions/06-loading-index.md` only when the needed files are unclear.

---

## Default Loading Rules

### Planning

Read:

- `instructions/01-execution-and-planning.md`
- `docs/project/PROMPT_TEMPLATE.md`
- relevant project memory docs

Use `$architecture-decision-check` when the proposed work may make or revise a durable architecture decision.

### Execution

Read:

- `instructions/01-execution-and-planning.md`
- `instructions/05-completion.md`
- relevant source, test, and documentation files

Use applicable repository Skills under `.agents/skills/` for repeatable workflow checks.

### Runtime Code Changes

Read:

- `instructions/01-execution-and-planning.md`
- `instructions/02-architecture-and-modules.md`
- `instructions/03-cqrs-database-testing-security.md`
- `instructions/05-completion.md`

### Documentation or Memory Changes

Read:

- `instructions/04-documentation-and-memory.md`
- `instructions/05-completion.md`

### Push Only

Read:

- `instructions/05-completion.md`
- `.agents/skills/push-readiness/SKILL.md`

### High-Risk or Unclear Tasks

Read all relevant instruction files and project memory.

High-risk includes:

- architecture
- workflow rules
- security
- authentication or authorization
- database or migrations
- CI/CD
- deployment
- Text-to-SQL safety
- assistant autonomy
- module boundaries

---

## Core Instruction Files

- `instructions/00-role-and-stack.md` — role, stack, repo structure, modules
- `instructions/01-execution-and-planning.md` — planning, approval, branch, execution rules
- `instructions/02-architecture-and-modules.md` — Clean Architecture, DDD, module boundaries
- `instructions/03-cqrs-database-testing-security.md` — CQRS, database, tests, security
- `instructions/04-documentation-and-memory.md` — docs, prompt logs, memory, ADRs
- `instructions/05-completion.md` — verification, CODE_REVIEW, commit, push, final report
- `instructions/06-loading-index.md` — optional helper for file-loading decisions

---

## Repository Skills

The canonical repository Skill root is `.agents/skills/`. Each discoverable Skill has one `.agents/skills/<skill-name>/SKILL.md` entrypoint.

- `architecture-decision-check` — determine whether work requires a new or updated ADR
- `branch-start-check` — verify safe execution startup on a dedicated branch
- `code-review-check` — review changes for defects, risk, and scope drift
- `commit-readiness` — check verified changes before an approved local commit
- `migration-safety-check` — review database changes for safety and approval
- `project-memory-update` — keep project memory aligned with verified state
- `prompt-log-writer` — create and finalize chronological prompt records
- `push-readiness` — check branch safety and explicit push approval
- `secret-scan-check` — detect credentials and sensitive values in changes
- `verification-runner` — select and report proportionate verification

Do not duplicate complete Skill instructions in this router.

---

## Key Project Files

Load only when needed:

- `docs/project/PROMPT_TEMPLATE.md`
- `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md`
- `docs/project/CODE_REVIEW.md`
- `docs/project/PROJECT_STATUS.md`
- `docs/project/AI_HANDOFF.md`
- `docs/project/ROADMAP.md`
- `docs/project/NEXT_SESSION.md`
- `.agents/skills/*/SKILL.md`
- `docs/agents/workflow/*`

---

## Guardrails

Skills and sub-agent docs are workflow guidance only. They are not approval.

They must not make execution, commit, push, PR creation, migration execution, deployment, destructive actions, or runtime AI behavior automatic.

When `AGENT.md` changes, provide and apply a full replacement `AGENT.md`.

When any instruction file changes, preserve still-valid rules and update project memory if operating rules change.

Prefer selective loading for small tasks. Prefer broader loading for risky or unclear tasks.
