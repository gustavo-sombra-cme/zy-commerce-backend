# Completion

## SELF REVIEW RULE

After execution the agent must review:

* Architecture compliance
* CQRS compliance
* Dependency compliance
* Naming consistency
* Build status
* Test status
* Security concerns

The review must be included in the execution summary.

---

# COMPLETION RULE

A task is complete only when:

* Build succeeds
* Tests pass
* Architecture remains compliant
* AGENT rules are respected
* Prompt log is updated
* Project memory is updated when project state changes
* Execution summary is produced

If any item is missing:

TASK STATUS = INCOMPLETE
