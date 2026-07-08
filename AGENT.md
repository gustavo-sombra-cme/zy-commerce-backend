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

Reusable prompt workflow:

* `docs/project/PROMPT_TEMPLATE.md`

Repo-local workflow architecture:

* `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md`
* `docs/project/CODE_REVIEW.md`

Repo-local workflow skills:

* `docs/skills/workflow/branch-start-check.md`
* `docs/skills/workflow/prompt-log-writer.md`
* `docs/skills/workflow/code-review-check.md`
* `docs/skills/workflow/commit-readiness.md`
* `docs/skills/workflow/push-readiness.md`
* `docs/skills/workflow/verification-runner.md`
* `docs/skills/workflow/secret-scan-check.md`
* `docs/skills/workflow/project-memory-update.md`
* `docs/skills/workflow/architecture-decision-check.md`
* `docs/skills/workflow/migration-safety-check.md`

Repo-local workflow sub-agent guidance:

* `docs/agents/workflow/planning-sub-agent.md`
* `docs/agents/workflow/execution-sub-agent.md`
* `docs/agents/workflow/code-review-sub-agent.md`
* `docs/agents/workflow/git-workflow-sub-agent.md`
* `docs/agents/workflow/documentation-sub-agent.md`
* `docs/agents/workflow/security-review-sub-agent.md`
* `docs/agents/workflow/test-verification-sub-agent.md`

Skill and sub-agent docs are workflow guidance only. They are not approval, and they must not make execution, commit, push, PR creation, migration execution, deployment, destructive actions, or runtime AI behavior automatic.

Short prompts such as "Plan next Catalog feature: Update Product Details" or "Execute approved feature: Update Product Details" must be expanded using the reusable prompt template, instruction files, project memory, recent prompt logs, and applicable repo-local workflow skill docs.

Stable rule references:

* `instructions/00-role-and-stack.md#role`
* `instructions/00-role-and-stack.md#stack`
* `instructions/00-role-and-stack.md#current-architecture-strategy`
* `instructions/00-role-and-stack.md#repository-structure`
* `instructions/00-role-and-stack.md#active-modules`
* `instructions/01-execution-and-planning.md#state-machine`
* `instructions/01-execution-and-planning.md#execution-lock`
* `instructions/01-execution-and-planning.md#planning-rules`
* `instructions/01-execution-and-planning.md#branch-workflow-rules`
* `instructions/02-architecture-and-modules.md#clean-architecture-rules`
* `instructions/02-architecture-and-modules.md#module-rules`
* `instructions/03-cqrs-database-testing-security.md#cqrs-rules`
* `instructions/03-cqrs-database-testing-security.md#database-rules`
* `instructions/03-cqrs-database-testing-security.md#testing-rules`
* `instructions/03-cqrs-database-testing-security.md#architecture-test-rules`
* `instructions/03-cqrs-database-testing-security.md#security-rules`
* `instructions/04-documentation-and-memory.md#prompt-logging-rule`
* `instructions/04-documentation-and-memory.md#ai-project-memory-rule`
* `instructions/04-documentation-and-memory.md#repo-local-workflow-skills-rule`
* `instructions/04-documentation-and-memory.md#next_sessionmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#project_statusmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#ai_handoffmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#roadmapmd-maintenance-rule`
* `instructions/04-documentation-and-memory.md#full-agentmd-replacement-rule`
* `instructions/04-documentation-and-memory.md#architecture-decision-record-rule`
* `instructions/04-documentation-and-memory.md#learning-journal-rule`
* `instructions/05-completion.md#self-review-rule`
* `instructions/05-completion.md#code-review-rule`
* `instructions/05-completion.md#completion-rule`

When `AGENT.md` changes, provide and apply a full replacement `AGENT.md`.

When any instruction file changes, preserve all still-valid rules and update project memory when project operating rules change.
