# Prompt 112: Assistant Product Availability And Count Execution

- **Prompt Number:** 112
- **Date:** 2026-07-30
- **Purpose:** Execute the approved read-only assistant capability for active Catalog product availability and counts by natural name, SKU, or search text.
- **Status:** APPROVED

## Full Prompt

> APPROVED: EXECUTE Assistant product availability and count by search text
>
> Use the approved plan.
>
> Implement the small read-only demo feature.
> Update demo slides and project memory after verification.
> Do not commit, push, or create a PR.
> Stop before commit and return TASK_STATUS

## Result Summary

Execution is paused at the branch-start check. Remote `main` is current at `978bd7d`, while the required product-comparison behavior exists only on `feature/assistant-product-comparison` at `450948c` and has not been merged into `main`. Explicit approval is required to create the availability/count task as a stacked branch from the product-comparison branch.
