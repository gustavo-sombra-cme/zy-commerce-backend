# E-Commerce Backend Agent Instructions (V2 Router)

This file is the entry point for agent behavior.

Read and follow these instruction files in order:

1. `instructions/00-role-and-stack.md`
2. `instructions/01-execution-and-planning.md`
3. `instructions/02-architecture-and-modules.md`
4. `instructions/03-cqrs-database-testing-security.md`
5. `instructions/04-documentation-and-memory.md`
6. `instructions/05-completion.md`

The instruction files preserve the full V2 rule set.

Stable rule references:

* `instructions/00-role-and-stack.md#role`
* `instructions/00-role-and-stack.md#stack`
* `instructions/00-role-and-stack.md#current-architecture-strategy`
* `instructions/00-role-and-stack.md#repository-structure`
* `instructions/00-role-and-stack.md#active-modules`
* `instructions/01-execution-and-planning.md#state-machine`
* `instructions/01-execution-and-planning.md#execution-lock`
* `instructions/01-execution-and-planning.md#planning-rules`
* `instructions/02-architecture-and-modules.md#clean-architecture-rules`
* `instructions/02-architecture-and-modules.md#module-rules`
* `instructions/03-cqrs-database-testing-security.md#cqrs-rules`
* `instructions/03-cqrs-database-testing-security.md#database-rules`
* `instructions/03-cqrs-database-testing-security.md#testing-rules`
* `instructions/03-cqrs-database-testing-security.md#architecture-test-rules`
* `instructions/03-cqrs-database-testing-security.md#security-rules`
* `instructions/04-documentation-and-memory.md#prompt-logging-rule`
* `instructions/04-documentation-and-memory.md#ai-project-memory-rule`
* `instructions/04-documentation-and-memory.md#next_sessionmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#project_statusmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#ai_handoffmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#roadmapmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#full-agentmd-replacement-rule`
* `instructions/04-documentation-and-memory.md#architecture-decision-record-rule`
* `instructions/04-documentation-and-memory.md#learning-journal-rule`
* `instructions/05-completion.md#self-review-rule`
* `instructions/05-completion.md#completion-rule`

When `AGENT.md` changes, provide and apply a full replacement `AGENT.md`.

When any instruction file changes, preserve all still-valid rules and update project memory when project operating rules change.
