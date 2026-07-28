# Prompt 109: Assistant Product Comparison Execution

- **Prompt Number:** 109
- **Date:** 2026-07-28
- **Purpose:** Implement and verify the approved read-only assistant capability for comparing two active Catalog products resolved by name, SKU, or search text.
- **Status:** FAILED

## Full Prompt

> APPROVED: EXECUTE Assistant product comparison by name or SKU
>
> Use docs/prompts/108-assistant-product-comparison-planning.md.
>
> Implement the approved read-only demo feature.
> Update demo slides and project memory after verification.
> Do not change frontend, MCP, Text-to-SQL, migrations, packages, or write/admin behavior.
> Do not commit, push, or create a PR.
> Stop before commit and return TASK_STATUS.

## Result Summary

Implemented the approved read-only product comparison flow on `feature/assistant-product-comparison`. Supported comparison questions route to the bounded Catalog agent, resolve each side through exact active-only searches, return safe empty or ambiguity choices without guessing, require trusted details for two unique products, and calculate cheaper/equal-price results from server-owned decimal prices. Focused Catalog-agent and routing verification passed.

Full verification did not pass because the latest `main` baseline still has two previously identified issues outside this task's approved scope: architecture project discovery scans the nested `.worktrees` directory and committed `Assistant:TextToSql:Enabled` is `true` while its safety test requires `false`. Restore and build passed; all Auth, Catalog, and Orders unit tests passed; 211 architecture tests passed and 9 failed from those baseline causes. The task explicitly prohibited Text-to-SQL changes and did not authorize verification-blocker cleanup, so demo slides and project memory were not updated and the task stopped before commit.
