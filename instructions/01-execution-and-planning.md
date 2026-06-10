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

Short execution prompts such as "Execute approved feature: Update Product Details" are not valid execution approval unless they include the explicit approval phrase:

APPROVED: EXECUTE

The execution lock remains mandatory for all file creation, file modification, code generation, command execution, scaffolding, migrations, and project changes.

Short prompts do not override architecture, DDD, CQRS, module isolation, documentation, prompt logging, testing, security, or completion rules.
