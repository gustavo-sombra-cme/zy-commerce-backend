# AI Skills And Sub-Agent Architecture

## Overview

This document defines the backend repository's project-wide AI skills and sub-agent architecture. It covers Codex/harness workflow guidance and future runtime assistant organization without changing runtime behavior.

Workflow skills in this repository are valid repository-local Codex Skills under `.agents/skills/`. They are not shared with the frontend repository and are not approval by themselves.

Catalog and Orders runtime assistant sub-agents are API-layer classes. Future runtime sub-agents must preserve that boundary unless a separately approved architecture change says otherwise. Catalog, Orders, and Auth modules must not know about agents, prompts, model planning, Text-to-SQL routing, provider diagnostics, or assistant-specific orchestration.

## Definitions: Skill vs Sub-Agent

A Skill is a focused reusable Codex capability with a dedicated directory, a required `SKILL.md` entrypoint, UI metadata in `agents/openai.yaml`, and only the supporting resources it needs.

A sub-agent is a broader responsibility boundary that may use one or more skills. In this repository, workflow sub-agents are Markdown guidance under `docs/agents/workflow/`.

Runtime sub-agents are future API-layer classes that coordinate assistant behavior while dispatching business reads through existing Application/CQRS handlers.

## Workflow Skills

The canonical Skill root is `.agents/skills/`. Each Skill directory is named in lowercase kebab case and contains exactly one required `SKILL.md` entrypoint.

Required backend workflow skills:

* `branch-start-check`
* `prompt-log-writer`
* `code-review-check`
* `commit-readiness`
* `push-readiness`
* `verification-runner`
* `project-memory-update`
* `architecture-decision-check`
* `secret-scan-check`
* `migration-safety-check`

These Skills provide repeated checks. They do not replace `AGENT.md`, `instructions/*`, or explicit user approval. Their entrypoints are:

* `.agents/skills/architecture-decision-check/SKILL.md`
* `.agents/skills/branch-start-check/SKILL.md`
* `.agents/skills/code-review-check/SKILL.md`
* `.agents/skills/commit-readiness/SKILL.md`
* `.agents/skills/migration-safety-check/SKILL.md`
* `.agents/skills/project-memory-update/SKILL.md`
* `.agents/skills/prompt-log-writer/SKILL.md`
* `.agents/skills/push-readiness/SKILL.md`
* `.agents/skills/secret-scan-check/SKILL.md`
* `.agents/skills/verification-runner/SKILL.md`

Only `architecture-decision-check` currently needs a bundled reference: `references/adr-review-checklist.md`. It loads that checklist when an architecture area is affected, an existing ADR may own the decision, or materiality is uncertain. The remaining Skills are concise enough to operate from `SKILL.md` and repository sources directly; none requires scripts or assets.

### Skill Catalog

| Directory / display name | Trigger summary | Supporting resources | Output contract |
|---|---|---|---|
| `architecture-decision-check` / Architecture Decision Check | Architecture, boundary, persistence, security, integration, contract, technology, or runtime AI decisions | `references/adr-review-checklist.md`; no scripts or assets | `ADR_ACTION: CREATE \| UPDATE \| NOT_REQUIRED \| BLOCKED` |
| `branch-start-check` / Branch Start Check | Start of an explicitly approved execution | No references, scripts, or assets | `BRANCH_START: PASS \| BLOCKED` |
| `code-review-check` / Code Review Check | Findings-first review or pre-commit review of behavior-affecting changes | Reads `docs/project/CODE_REVIEW.md`; no bundled resources | `CODE_REVIEW: PASS \| BLOCKED` |
| `commit-readiness` / Commit Readiness | Immediately before an explicitly approved local commit | Consumes existing branch, scope, review, verification, secret, applicable migration, prompt-log, and memory evidence | `COMMIT_READINESS: PASS \| BLOCKED` |
| `migration-safety-check` / Migration Safety Check | Schema, migration, SQL, migration-permission, execution, or database-target/ownership work; not credential-only handling | Reads repository database guidance; no bundled resources | Plan safety, execution approval, and `MIGRATION_SAFETY: PASS \| FAIL \| BLOCKED \| NOT_APPLICABLE` |
| `project-memory-update` / Project Memory Update | Verified work changes persistent project state or operating constraints | Reads only affected project-memory files; no bundled resources | `PROJECT_MEMORY_UPDATE: PASS \| BLOCKED` |
| `prompt-log-writer` / Prompt Log Writer | Repository planning, execution, artifact-producing testing, documentation, Skill maintenance, or review unless explicitly skipped | Reads prompt-log rules; no bundled resources | `PROMPT_LOG: PASS \| BLOCKED` |
| `push-readiness` / Push Readiness | Immediately before a requested push | Consumes commit-readiness, executed-verification, and secret-scan evidence | `PUSH_READINESS: PASS \| BLOCKED` |
| `secret-scan-check` / Secret Scan Check | Before commit/push or whenever credential handling changes | Reads repository security guidance; no bundled resources | `SECRET_SCAN_STATUS: PASS \| BLOCKED` |
| `verification-runner` / Verification Runner | Dry-run planning when requested, or executed verification after changes | Reads repository verification guidance; no bundled resources | `VERIFICATION_DRY_RUN: COMPLETE \| BLOCKED` or `VERIFICATION_STATUS: PASS \| FAIL \| BLOCKED` |

Every Skill directory under `.agents/skills/` contains its canonical `SKILL.md` entrypoint and `agents/openai.yaml` UI metadata file.

## Workflow Sub-Agents

Workflow sub-agent guidance lives under `docs/agents/workflow/`.

Recommended workflow sub-agents:

* Planning Sub-Agent
* Execution Sub-Agent
* Code Review Sub-Agent
* Git Workflow Sub-Agent
* Documentation Sub-Agent
* Security Review Sub-Agent
* Test Verification Sub-Agent

These are responsibility profiles for Codex workflow, not independent runtime services.

## Code Review / Commit / Push Model

`docs/project/CODE_REVIEW.md` is mandatory before every commit that includes code, project/configuration files, migrations, CI workflow files, or runtime-behavior documentation changes.

Documentation-only maintenance changes are excluded from mandatory code review, but still require documentation self-review.

Commit readiness and push readiness are separate. A readiness skill may recommend committing or pushing, but it is not approval.

Push must remain explicitly human-approved. Both of these approval phrases are valid:

* `APPROVED: PUSH`
* `APPROVED: PUSH BACKEND BRANCH`

## API-Layer Runtime Sub-Agent Decision

Runtime assistant sub-agents should remain API-layer classes. They may coordinate assistant-facing use cases such as Catalog product discovery, Orders history analysis, safety validation, and unsupported request handling.

The Catalog runtime sub-agent is a bounded autonomous exception to deterministic sequencing, governed by ADR-007. Its model may propose calls only to the Catalog-specific registry (`catalog_search_products`, `catalog_get_product`). Server code validates every argument, enforces public active-only scope and hard limits, records trusted product identifiers per execution, and reconstructs final product contracts from CQRS results. Product text and model output are untrusted. This pattern does not authorize autonomy in Orders, Auth, Text-to-SQL, MCP, or write workflows.

They must dispatch real business reads through existing Application/CQRS handlers, usually through `ISender`.

They must not call EF Core DbContexts, repositories, Domain objects, module internals, write commands, MCP protocol types, raw SQL paths, or provider clients directly unless an explicitly approved architecture change says otherwise.

## Why Sub-Agents Stay In API Layer

The assistant is a platform orchestration concern. It translates natural-language user requests into safe backend reads and response shapes.

Catalog owns product lifecycle and product reads. Orders owns order history. Auth owns identity and roles. Those modules should not know about prompts, model plans, Text-to-SQL routing, provider diagnostics, assistant fallback behavior, or unsupported natural-language handling.

Keeping runtime sub-agents in the API layer preserves Clean Architecture:

* Domain remains independent.
* Application owns use cases through commands, queries, handlers, validators, DTOs, and abstractions.
* Infrastructure owns persistence and external concerns.
* API coordinates transport, authenticated user context, assistant orchestration, and safe response mapping.

## Why Text-to-SQL Is Not Moved Now

Text-to-SQL remains as-is for now. Do not turn it into a skill in the current migration.

Current Text-to-SQL safety depends on the existing feature flag, approved assistant views, SQL validator, read-only connection strings, executor, response mapper, and fallback to the existing CQRS assistant flow.

Moving it too early would increase risk without improving safety. A future phase may make Text-to-SQL a selectable strategy with explicit telemetry, but that should be planned and tested separately.

## What Must Not Become A Skill

The following must not become automatic skills:

* Push, PR creation, PR merge, or production deployment.
* Secret rotation or secret creation.
* Database migration execution.
* Destructive Git or filesystem actions.
* Admin/write assistant actions.
* Runtime Catalog, Orders, or Auth business rules.
* Broad refactoring.
* Text-to-SQL conversion in this task.

## Migration Roadmap

1. Document the repo-local AI skills and sub-agent architecture.
2. Add reusable repository-local Codex Skills and workflow sub-agent guidance.
3. Update router, instructions, prompt template, and project memory to reference the docs.
4. Keep runtime assistant code unchanged.
5. Keep the implemented Catalog and Orders runtime sub-agent classes in the API layer and evaluate any additional runtime sub-agent separately.
6. Later, consider a selectable Text-to-SQL strategy with explicit telemetry.
7. Defer any admin/support assistant sub-agents until separate ADRs, tests, and authorization rules are approved.

## Risks

* Duplicating policy between instructions and skill docs.
* Treating readiness output as approval.
* Making push too automatic.
* Weakening code review or secret scanning.
* Moving runtime assistant logic into modules and violating module isolation.
* Weakening Text-to-SQL safety by moving it before the strategy is stable.
* Changing frontend contracts or assistant response shapes.
* Introducing write/admin AI behavior too early.

## Open Future Work

* Decide whether any additional runtime sub-agent is justified beyond the implemented Catalog and Orders sub-agents.
* Decide telemetry shape for future selectable Text-to-SQL strategy.
* Decide whether future repository Skills need additional scenarios without sharing a common backend/frontend Skill set.
* Extend architecture tests when additional runtime sub-agent boundaries are introduced or existing boundaries change.
