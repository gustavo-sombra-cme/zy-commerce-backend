# Documentation And Memory

## PROMPT LOGGING RULE

All architecture, planning, execution, testing, documentation, and review prompts must be logged.

Location:

docs/prompts/

Naming format:

001-title.md
002-title.md
003-title.md

Each prompt log must contain:

* Prompt Number
* Date
* Purpose
* Full Prompt
* Status
* Result Summary

Statuses:

* PLANNED
* APPROVED
* EXECUTED
* FAILED

If docs/prompts does not exist:

Create it automatically.

Prompt logging must occur before execution begins.

The only exception is when the user explicitly writes:

SKIP PROMPT LOG

Historical prompt logs must not be rewritten for template cleanup or style normalization.

Reusable prompt guidance belongs in:

docs/project/PROMPT_TEMPLATE.md

Do not place reusable templates inside `docs/prompts/`; that directory is for chronological prompt logs.

---

# AI PROJECT MEMORY RULE

The repository is the persistent source of truth for future AI sessions.

Project memory lives in:

docs/project/

Required files:

* PROJECT_STATUS.md
* AI_HANDOFF.md
* ROADMAP.md
* NEXT_SESSION.md

After every completed execution task, update these files when the task changes:

* solution structure
* project references
* modules
* architecture tests
* APIs
* business features
* database schema or migrations
* packages
* testing status
* roadmap status
* known risks
* operating constraints

Project memory must be factual, concise, and aligned with the current repository.

Do not record speculative work as completed.

---

# FEATURE DEMO SLIDE DELIVERABLE RULE

For every completed execution task that implements or materially changes a main feature, major platform capability, API module, integration, or demo-worthy backend behavior, create or update a presentation-ready Markdown slide source file.

Location:

docs/demo/features/

Naming format:

{feature-slug}-demo-slides.md

Examples:

* mcp-server-integration-demo-slides.md
* orders-list-current-user-demo-slides.md
* catalog-product-price-write-support-demo-slides.md

This deliverable is not required for tiny fixes, typo fixes, internal refactors with no demo value, prompt-template-only cleanup, test-only cleanup, or documentation-only maintenance unless the user explicitly requests it.

Each feature demo slide file must include, when applicable:

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

Use Mermaid diagrams where useful:

* Architecture diagram
* Sequence diagram
* Data flow diagram when applicable

Every slide should include a short `Speaker cue:` line.

The execution summary must mention the generated or updated feature demo slide file. If a slide file is not required, the execution summary must state why.

When the feature changes project state, update project memory to reference the demo slide file where useful.

Reusable demo slide guidance belongs in project documentation. Do not place reusable slide templates in `docs/prompts/`; that directory is for chronological prompt logs.

---

# NEXT_SESSION.md MAINTENANCE RULE

NEXT_SESSION.md is a fast resume guide optimized for new sessions with no conversation history.

Location:

docs/project/

Purpose:

* Allow future AI sessions to resume in less than 5 minutes.
* Capture last completed work.
* Capture current repository state.
* Capture current architecture.
* Capture current phase.
* Capture next approved task.
* Provide required reading order.
* Include critical warnings and constraints.
* Provide quick verification commands.

After every completed execution task, update:

* Last Completed Work
* Current Repository State (if changed)
* Current Architecture (if changed)
* Current Phase (if changed)
* Next Approved Task (if user provides new approval)
* Build & Test Status (if changed)
* Warnings (if new constraints discovered)

---

# PROJECT_STATUS.md MAINTENANCE RULE

PROJECT_STATUS.md must answer:

What exists right now?

Maintain:

* current solution structure
* active modules and their maturity
* implemented features
* database status
* API status
* architecture rules currently enforced
* latest known build and test status
* intentionally absent features

Update this file after execution tasks that change project state.

---

# AI_HANDOFF.md MAINTENANCE RULE

AI_HANDOFF.md must answer:

What does the next AI session need to know before touching the repository?

Maintain:

* recommended reading order
* architecture constraints
* module boundaries
* current implementation notes
* gotchas and warnings
* verification commands
* prompt log and ADR locations
* recent completed work

Optimize this file for future sessions with limited chat history.

---

# ROADMAP.md MAINTENANCE RULE

ROADMAP.md must answer:

What is completed, and what may come next?

Maintain separate sections for:

* completed work
* current priorities
* candidate future work
* explicitly not-started work

Move items from future work to completed only after execution is complete.

Do not invent committed scope.

---

# FULL AGENT.md REPLACEMENT RULE

When AGENT.md changes, provide and apply a full replacement AGENT.md.

Do not make partial AGENT.md edits.

The replacement must preserve all still-valid rules and explicitly include any new rules requested by the user.

---

# ARCHITECTURE DECISION RECORD RULE

Major architectural decisions must be recorded.

Location:

docs/decisions/

Format:

ADR-001-title.md
ADR-002-title.md

Each ADR must contain:

* Context
* Options Considered
* Decision
* Rationale
* Consequences

---

# LEARNING JOURNAL RULE

When requested by the user, record lessons learned.

Location:

docs/learning-journal/

Purpose:

* Capture architectural insights
* Capture mistakes
* Capture tradeoffs
* Capture reasoning behind decisions
