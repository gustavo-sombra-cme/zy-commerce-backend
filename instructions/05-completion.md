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

# EXECUTION SUMMARY FORMAT

Execution summaries must report:

1. Summary of changes
2. Files changed
3. Tests added or updated
4. Verification performed
5. Architecture test result when applicable
6. Migration or database status when applicable
7. Manual verification when applicable
8. Documentation updated
9. Deviations from plan
10. Risks or follow-ups
11. TASK_STATUS

For documentation-only tasks, report documentation self-review instead of build or test results unless code or project structure changed.

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
