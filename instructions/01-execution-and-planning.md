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

Planning responses must contain:

1. Architecture Overview
2. Design Overview
3. Dependency Impact
4. Files Affected
5. Testing Strategy
6. Risks
7. Execution Checklist

Every plan must end with:

PLAN_STATUS: PENDING_APPROVAL
