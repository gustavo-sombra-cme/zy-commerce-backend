# AI Skills And Sub-Agent Architecture

## Overview

This document defines the backend repository's project-wide AI skills and sub-agent architecture. It covers Codex/harness workflow guidance and future runtime assistant organization without changing runtime behavior.

Workflow skills in this repository are repo-local Markdown only. They are not installed Codex skills, not shared with the frontend repository, and not approval by themselves.

Runtime assistant sub-agents, if introduced later, remain API-layer classes. Catalog, Orders, and Auth modules must not know about agents, prompts, model planning, Text-to-SQL routing, provider diagnostics, or assistant-specific orchestration.

## Definitions: Skill vs Sub-Agent

A skill is a focused reusable capability with clear inputs, outputs, rules, stop conditions, and verification expectations.

A sub-agent is a broader responsibility boundary that may use one or more skills. In this repository, workflow sub-agents are Markdown guidance under `docs/agents/workflow/`.

Runtime sub-agents are future API-layer classes that coordinate assistant behavior while dispatching business reads through existing Application/CQRS handlers.

## Workflow Skills

Workflow skills live under `docs/skills/workflow/`.

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

These skill docs are references for repeated checks. They do not replace `AGENT.md`, `instructions/*`, or explicit user approval.

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
2. Add reusable workflow skill docs and workflow sub-agent guidance.
3. Update router, instructions, prompt template, and project memory to reference the docs.
4. Keep runtime assistant code unchanged.
5. Later, consider API-layer runtime sub-agent classes around existing assistant orchestration.
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

* Decide whether runtime sub-agent classes should be introduced around the current `AssistantOrchestrator`.
* Decide telemetry shape for future selectable Text-to-SQL strategy.
* Decide whether workflow skill docs need examples for frontend/backend parity without sharing a common skill set.
* Add architecture tests if runtime sub-agent classes are introduced.
