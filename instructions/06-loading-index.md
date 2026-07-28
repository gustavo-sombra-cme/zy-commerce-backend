# Loading Index

`AGENT.md` is the always-loaded repository routing baseline. Use this index only when `AGENT.md` does not provide enough detail to choose conditional instructions, Skills, project memory, ADRs, or workflow guidance.

Selective loading is the default. Do not read every instruction, ADR, memory file, Skill, or workflow document unless the task is broad, unclear, high-risk, or the user explicitly requests a comprehensive review.

## Routing layers

1. **Always loaded:** `AGENT.md`.
2. **Conditional instruction:** load the smallest relevant file under `instructions/`.
3. **Skill metadata:** use the available Skill name and description to decide whether the Skill applies.
4. **Skill body:** after a Skill applies, read its full canonical `SKILL.md` entrypoint under `.agents/skills/` before acting.
5. **Skill references:** load only references the selected `SKILL.md` requires for the current surface.
6. **Task evidence:** load only relevant source, tests, configuration, memory, ADRs, or workflow guidance.

Do not load a Skill merely to mention that it was skipped. Do not load scope-specific references for an unrelated changed file.

## Global prompt logging route

Load `.agents/skills/prompt-log-writer/SKILL.md` before:

- repository planning;
- approved execution;
- testing that creates or changes a repository artifact;
- documentation changes;
- repository Skill maintenance;
- findings-first repository review; or
- global workflow-policy work.

Do not load it for general explanation, advice, or read-only questions that create no repository artifact, or when the user explicitly writes `SKIP PROMPT LOG`.

Creating a planning prompt log is a governance artifact, not authorization to implement the plan.

## Task type to conditional files

| Task type | Load |
|---|---|
| Planning | `instructions/01-execution-and-planning.md`, `docs/project/PROMPT_TEMPLATE.md`, prompt-log Skill, relevant memory and source |
| Approved execution | `instructions/01-execution-and-planning.md`, `instructions/05-completion.md`, branch-start and prompt-log Skills, relevant source/test/docs |
| Runtime code | `instructions/01-execution-and-planning.md`, `instructions/02-architecture-and-modules.md`, `instructions/03-cqrs-database-testing-security.md`, `instructions/05-completion.md`, relevant source/tests |
| Documentation change | `instructions/04-documentation-and-memory.md`, relevant docs, prompt-log Skill; verification Skill after edits |
| Commit readiness | `instructions/01-execution-and-planning.md`, `instructions/05-completion.md`, `.agents/skills/commit-readiness/SKILL.md`, and existing review/verification/secret/migration/log/memory evidence |
| Push readiness | `instructions/01-execution-and-planning.md`, `instructions/05-completion.md`, `.agents/skills/push-readiness/SKILL.md`, and existing commit/verification/secret evidence |
| Architecture planning or review | `instructions/02-architecture-and-modules.md`, relevant source and only possibly governing ADRs; architecture-decision Skill when a decision may be created or changed |
| Security or auth | `instructions/03-cqrs-database-testing-security.md`, relevant source/tests/config, secret-scan Skill when changed material may contain secrets |
| Database-impacting work | `instructions/03-cqrs-database-testing-security.md`, `.agents/skills/migration-safety-check/SKILL.md`, relevant schema/migrations/SQL/target configuration |
| Credential-only concern | security guidance and `.agents/skills/secret-scan-check/SKILL.md`; do not load migration safety unless target, ownership, permission, schema, migration, or SQL behavior changes |
| Workflow or Skill maintenance | `instructions/04-documentation-and-memory.md`, relevant Skill bodies and metadata, project architecture guidance when source-of-truth behavior changes |

## Instruction file purpose

- `instructions/00-role-and-stack.md` — stack, repository structure, and module context.
- `instructions/01-execution-and-planning.md` — planning, execution, branch, approval, and dirty-worktree rules.
- `instructions/02-architecture-and-modules.md` — Clean Architecture, DDD, module boundaries, public contracts, dependency direction, and runtime AI ownership.
- `instructions/03-cqrs-database-testing-security.md` — CQRS, database, migration, test, authentication, authorization, and secret-handling rules.
- `instructions/04-documentation-and-memory.md` — prompt logs, project memory, ADRs, documentation, and workflow-Skill governance.
- `instructions/05-completion.md` — verification, review, commit, push, and completion reporting.

## Conditional Skill routes

- `.agents/skills/architecture-decision-check/SKILL.md` — a proposal or diff may create or change an architectural decision.
- `.agents/skills/branch-start-check/SKILL.md` — before implementation for an `APPROVED: EXECUTE` task.
- `.agents/skills/code-review-check/SKILL.md` — findings-first review or pre-commit review for applicable changed surfaces.
- `.agents/skills/commit-readiness/SKILL.md` — immediately before an explicitly approved local commit.
- `.agents/skills/migration-safety-check/SKILL.md` — database-impacting proposals or changes, including target or ownership changes; not credential-only concerns.
- `.agents/skills/project-memory-update/SKILL.md` — verified persistent repository state changed.
- `.agents/skills/prompt-log-writer/SKILL.md` — governed repository work listed in the global prompt logging route.
- `.agents/skills/push-readiness/SKILL.md` — only before an explicitly requested push.
- `.agents/skills/secret-scan-check/SKILL.md` — before every commit and push readiness, or when configuration or credential handling changes.
- `.agents/skills/verification-runner/SKILL.md` — verification dry run when requested, or executed verification after changes.

## Architecture and ADR loading

Load `instructions/02-architecture-and-modules.md` for explicit architecture-compliance review or when the proposed or changed surface can affect architecture. Load only relevant ADRs when the task changes architecture, explicitly requests ADR compliance, or identifies a possibly governing ADR. Do not enumerate every ADR by default.

## Project memory loading

Load `.agents/skills/project-memory-update/SKILL.md` only after a verified persistent state change. Then load only affected memory files:

- `docs/project/PROJECT_STATUS.md` — what currently exists and verified build/test state.
- `docs/project/AI_HANDOFF.md` — constraints, gotchas, and operating guidance for the next session.
- `docs/project/ROADMAP.md` — completed, current-priority, candidate, and not-started work.
- `docs/project/NEXT_SESSION.md` — fast-resume state, last completed work, next approved task, commands, and warnings.

Plans, hypothetical or unverified work, grammar-only edits, and temporary test artifacts do not trigger project-memory updates.

## Broad-loading conditions

Read more broadly when the task spans multiple modules or materially affects architecture, workflow policy, authentication or authorization, database ownership or migrations, CI/CD, deployment, Text-to-SQL safety, runtime assistant autonomy, or agent orchestration. Broad loading must still be relevant to the requested surface.

## Reporting rule

When useful, report actual routing decisions in this form:

```text
Loaded:
- exact path — reason

Skipped:
- exact path — reason
```
