# Reusable Prompt Template

Use this template to expand short prompts into complete planning or execution instructions.

Examples:

* Plan next Catalog feature: Update Product Details
* Execute approved feature: Update Product Details
* Plan Auth phase: Refresh Token Strategy
* Execute approved fix: Swagger Authorization Header

## Required Reading

Before planning or execution, read:

1. `AGENT.md`
2. `instructions/*`
3. `docs/project/PROJECT_STATUS.md`
4. `docs/project/AI_HANDOFF.md`
5. `docs/project/ROADMAP.md`
6. `docs/project/NEXT_SESSION.md`
7. Latest relevant files in `docs/prompts/`
8. Relevant ADRs in `docs/decisions/`

---

# PLAN MODE Defaults

When the prompt starts with `PLAN MODE` or `Plan ...`:

* Do not create code.
* Do not modify files unless the user explicitly asks for documentation updates.
* Do not run build, test, EF, scaffold, or migration commands.
* Inspect repository context only as needed.
* Identify architecture, DDD, CQRS, database, API, testing, and documentation impact.
* Include explicit in-scope and out-of-scope work.
* Explain architectural rationale, not only implementation steps.
* Explain why the design was chosen.
* Explain why important alternatives were not selected.
* Identify which DDD principles are being applied.
* Identify which Clean Architecture boundaries are involved.
* Call out tradeoffs introduced by the chosen design.
* Use the Plan Output Contract exactly.
* Run the Plan Self-Validation Rule before returning the plan.
* End with `PLAN_STATUS: PENDING_APPROVAL`.

## Planning Requirements

Planning must not only describe WHAT will be implemented.

Planning must also explain:

* WHY the design was chosen.
* WHY alternative approaches were rejected.
* Which DDD principles are being applied.
* Which Clean Architecture boundaries are involved.
* Any important tradeoffs.
* Potential future evolution paths.

---

# Mandatory Plan Sections

The required plan output sections are mandatory.

Use the exact section names shown in the Plan Output Contract.

Do not:

* Rename sections
* Merge sections
* Omit sections
* Reorder sections
* Replace explicit sections with condensed bullets

If a section is not applicable:

* Include the section anyway.
* Explain why it is not applicable.

Architectural Rationale and DDD Ownership are required for all feature plans.

## Plan Output Contract

For every PLAN MODE response and every short prompt beginning with `Plan ...`, the final answer MUST include every section below, using the exact heading names.

Do not omit, rename, merge, reorder, or collapse these sections.

If a section is not applicable, include the section and write `Not applicable because ...`.

A plan that omits any required section is non-compliant and must be corrected before returning the response.

## Plan Quality Standard

A complete plan must include:

* Architectural decisions
* Architectural rationale
* DDD ownership boundaries
* CQRS responsibilities
* API impact
* Database impact
* Documentation impact
* Testing strategy
* Verification strategy
* Risks
* Follow-ups

A plan that omits any required section is incomplete.

The goal is not only to describe WHAT will be implemented but also:

* WHY it will be implemented this way
* WHY alternative approaches were rejected
* HOW the design aligns with Clean Architecture
* HOW the design aligns with DDD

## Required Plan Output

Every plan MUST use this exact output structure:

# Architecture Overview

# Design Overview

# Architectural Rationale

# DDD Ownership

# CQRS Design

# API / Contract Impact

# Database / Migration Impact

# Files Affected

# Documentation Updates

# Testing Strategy

# Verification Plan

# Risks

# Follow-ups

# Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Plan Self-Validation Rule

Before returning any plan, perform an internal self-validation pass.

Verify that the final response contains these exact sections:

* Architecture Overview
* Design Overview
* Architectural Rationale
* DDD Ownership
* CQRS Design
* API / Contract Impact
* Database / Migration Impact
* Files Affected
* Documentation Updates
* Testing Strategy
* Verification Plan
* Risks
* Follow-ups
* Execution Checklist

If any section is missing, add it before returning the plan.

The final visible response does not need to include the checklist results unless the user asks for them.

## Planning Compliance Checklist

Before returning a plan, confirm:

* The response is planning-only.
* No code or file modifications are proposed as already completed.
* Every required heading exists exactly once.
* Architectural Rationale explains why the design is preferred.
* DDD Ownership identifies aggregate and module responsibility.
* CQRS Design makes command/query responsibilities explicit.
* API / Contract Impact is stated.
* Database / Migration Impact is stated.
* Documentation Updates are stated.
* Testing Strategy and Verification Plan are separate.
* Risks and Follow-ups are separate.
* Execution Checklist is actionable.
* The plan ends with `PLAN_STATUS: PENDING_APPROVAL`.

## Example Compliant Plan Structure

```md
# Architecture Overview

...

# Design Overview

...

# Architectural Rationale

...

# DDD Ownership

...

# CQRS Design

...

# API / Contract Impact

...

# Database / Migration Impact

...

# Files Affected

...

# Documentation Updates

...

# Testing Strategy

...

# Verification Plan

...

# Risks

...

# Follow-ups

...

# Execution Checklist

1. ...

PLAN_STATUS: PENDING_APPROVAL
```

---

# APPROVED EXECUTE Defaults

When the prompt starts with `APPROVED: EXECUTE` or `Execute approved ...`:

* Execution is allowed only for the approved scope.
* Follow the Branch Workflow Rules in `instructions/01-execution-and-planning.md` before changing files.
* Use applicable repo-local workflow skills in `docs/skills/workflow/` for repeated workflow checks.
* Create or update the planning prompt log and create the execution prompt log before implementation, unless `SKIP PROMPT LOG` is present.
* Follow the approved plan exactly.
* Do not add unapproved modules, projects, migrations, schema changes, APIs, packages, auth behavior, or cross-module references.
* Update project memory after completed execution when project state changes.
* Create or update the feature demo slide deliverable when the approved scope is a main feature, major platform capability, API module, integration, or demo-worthy backend behavior.
* Run standard verification for code or project changes.

## Default Verification Commands

For code or project changes:

```text
dotnet restore Ecommerce.sln
dotnet build Ecommerce.sln
dotnet test Ecommerce.sln
```

For documentation-only tasks:

* Perform documentation self-review.
* Do not run build or test commands unless code or project structure changes are required.

## Branch Workflow Summary

Every approved execution task must use one task, one branch, and one PR. Start from latest `main`, confirm repository path, branch, and `git status --short --branch`, stop on a dirty worktree unless the user approves including the changes or using a separate worktree, then create a dedicated `feature/`, `fix/`, `docs/`, or `chore/` branch before implementation.

Do not work directly on `main`, push directly to `main`, commit automatically, push automatically, or create a pull request automatically. After implementation and verification, wait for one of:

* `APPROVED: COMMIT BACKEND CHANGES`
* `APPROVED: PUSH`
* `APPROVED: PUSH BACKEND BRANCH`
* `APPROVED: CREATE BACKEND PR`
* `APPROVED: COMMIT AND PUSH BACKEND CHANGES`

Readiness skills can recommend commit or push readiness, but they are never approval by themselves.

Add manual verification when API, auth, database, Swagger, or behavior-visible changes are involved.

## Default Execution Output

1. Files Changed
2. Implementation Summary
3. Tests Added / Updated
4. Verification Results
5. Architecture Test Result
6. Migration / Database Status
7. Manual Verification Result (if applicable)
8. Documentation Updated
9. Deviations From Plan
10. Risks / Follow-ups
11. TASK STATUS

## Feature Demo Slide Deliverable

For every approved execution task that implements or materially changes a main feature, major platform capability, API module, integration, or demo-worthy backend behavior, create or update a presentation-ready Markdown slide source file.

Default location:

```text
docs/demo/features/{feature-slug}-demo-slides.md
```

Examples:

```text
docs/demo/features/mcp-server-integration-demo-slides.md
docs/demo/features/orders-list-current-user-demo-slides.md
docs/demo/features/catalog-product-price-write-support-demo-slides.md
```

The slide deliverable is not required for tiny fixes, typo fixes, internal refactors with no demo value, prompt-template-only cleanup, test-only cleanup, or documentation-only maintenance unless the user explicitly requests it.

Each feature demo slide file must be structured as slide-ready Markdown and include, when applicable:

* Feature title
* Business purpose
* Problem solved
* Architecture overview
* Implementation files
* API/contracts involved
* Database impact
* Security/authorization behavior
* Main sequence diagram
* Demo script
* Test evidence
* Risks/tradeoffs
* Q&A talking points

Diagram requirements:

* Include Mermaid architecture diagrams when they clarify component boundaries.
* Include Mermaid sequence diagrams for the main user/system flow.
* Include Mermaid data-flow diagrams when persistence, integration, or tool orchestration is central to the feature.

Speaker notes:

* Every slide should include a short `Speaker cue:` line.
* Speaker cues should explain what to say live, what backend behavior to point out, and what risk/tradeoff to emphasize.

Execution summary requirement:

* The final execution summary must mention the feature demo slide file path when one is created or updated.
* If no slide file is required, the execution summary must state why it was not required.

Project memory requirement:

* When the feature changes project state, update project memory to reference the demo slide deliverable where useful.
* Do not place reusable slide templates in `docs/prompts/`; prompt logs remain chronological historical records only.

---

# Learning Mode

If the task is educational, architectural, DDD-related, CQRS-related, or Clean Architecture-related:

* Explain important DDD concepts being applied.
* Explain Clean Architecture boundaries involved.
* Explain CQRS responsibilities involved.
* Explain tradeoffs and alternative approaches when relevant.
* Prefer teaching over only prescribing implementation steps.
* Call out common mistakes and anti-patterns.
* Explain why a particular design is preferred.
* Explain how the solution aligns with the project's architecture strategy.

---

# Architecture Defaults

* Preserve Clean Architecture dependency direction.
* Domain remains independent.
* Application must not depend on API or Infrastructure.
* Infrastructure implements persistence and external concerns.
* API controllers remain thin.
* BuildingBlocks must not reference modules.
* Modules must remain isolated.
* Preserve existing architecture tests.
* Do not create Bootstrapper, Shared, microservices, event bus, or distributed transaction patterns without explicit approval and ADR.

---

# DDD Defaults

* Each module owns its own Domain, Application, Infrastructure, Contracts, and persistence.
* Aggregates own business state transitions and invariants.
* Value objects remain in Domain.
* Application coordinates use cases through commands, queries, handlers, validators, DTOs, and abstractions.
* Infrastructure persists module-owned models and implements external concerns.
* Modules do not reference each other's internals.
* Business rules belong in aggregates and domain models, not controllers.
* Controllers must not contain business logic.
* Handlers orchestrate use cases but should not own domain behavior.

---

# CQRS Defaults

* Writes use Command, CommandHandler, and Validator.
* Reads use Query, QueryHandler, and DTO.
* Do not mix read and write responsibilities.
* Prefer DTO projection for reads.
* Controllers dispatch commands and queries, then return responses.
* Read models may differ from aggregate persistence models when justified.
* Query-side optimizations must not leak into Domain behavior.

---

# Documentation Defaults

For execution tasks that change project state, update as applicable:

* `docs/project/PROJECT_STATUS.md`
* `docs/project/AI_HANDOFF.md`
* `docs/project/ROADMAP.md`
* `docs/project/NEXT_SESSION.md`

Feature demo slide deliverables go in:

```text
docs/demo/features/
```

Use the naming convention:

```text
{feature-slug}-demo-slides.md
```

For main feature, major platform capability, API module, integration, or demo-worthy backend behavior executions, create or update the relevant feature demo slide file and reference it in the execution summary. If the task does not require a slide file, state why in the execution summary.

Prompt logs go in:

```text
docs/prompts/
```

using the next available prompt number.

Major architecture decisions require an ADR in:

```text
docs/decisions/
```

Prompt logs are historical records and should not be rewritten after completion.

Reusable templates belong in project documentation, not prompt history.

Repo-local workflow skill docs live in:

```text
docs/skills/workflow/
```

Workflow sub-agent guidance lives in:

```text
docs/agents/workflow/
```

Use these docs for reusable workflow checks instead of duplicating large checklist sections in prompts or instruction files.

---

# Global Architectural Restrictions

Unless explicitly approved:

* New modules
* New projects
* Cross-module references
* Bootstrapper projects
* Shared projects
* Microservices
* Event bus
* Distributed transactions

---

# Current Project Restrictions

Unless explicitly approved:

* Migrations
* Database schema changes
* Startup auto-migrations
* API endpoints
* Package additions
* Refresh tokens
* Broader permissions beyond current Customer/Admin role support
* Token persistence
* Customers module
* Protected Catalog read endpoints

If a requested task conflicts with these restrictions:

* Stop.
* Explain the conflict.
* Request explicit approval before proceeding.

---

# Short Prompt Expansion Rules

Short prompts MUST be expanded into the full PLAN MODE or APPROVED EXECUTE contract.

A short planning prompt such as:

```text
Plan next Catalog feature: Update Product Details
```

means:

* Use PLAN MODE.
* Apply all required reading.
* Apply all architecture, DDD, CQRS, documentation, verification, and restriction defaults.
* Return the full Required Plan Output structure exactly.
* Run the Plan Self-Validation Rule before responding.
* End with `PLAN_STATUS: PENDING_APPROVAL`.

A short execution prompt is only valid when it includes the explicit approval phrase:

```text
APPROVED: EXECUTE
```

Examples:

```text
Plan next Catalog feature: Update Product Details
```

```text
Plan next Auth feature: Refresh Token Strategy
```

```text
Plan API improvement: Product Search Optimization
```

```text
APPROVED: EXECUTE Update Product Details
```

```text
APPROVED: EXECUTE Swagger Authorization Fix
```

The AI MUST automatically apply:

* Architecture defaults
* DDD defaults
* CQRS defaults
* Documentation defaults
* Verification defaults
* Project restrictions
* Learning Mode behavior
* Plan Output Contract
* Plan Self-Validation Rule

without requiring those instructions to be repeated in every prompt.
