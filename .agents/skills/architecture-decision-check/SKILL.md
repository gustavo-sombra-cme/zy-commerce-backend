---
name: architecture-decision-check
description: Reviews proposed implementation and design changes to determine whether an Architecture Decision Record must be created or updated. Use when planning, executing, or reviewing work that may affect architecture, bounded-context boundaries, dependency direction, security, persistence, external integrations, deployment, public contracts, runtime AI autonomy, agent orchestration, or technology choices. Do not use for one-time writing or formatting work with no architectural effect.
---

# Architecture Decision Check

## Required input

Obtain the proposed change, its affected areas or diff when available, and repository access. If the request is too vague and repository inspection cannot resolve it, return `ADR_ACTION: BLOCKED`.

## Workflow

1. Read `AGENT.md`, the ADR rule in `instructions/04-documentation-and-memory.md`, current project memory, and relevant architecture guidance.
2. Inspect the proposed plan, diff, or affected files. Identify changes to module ownership, dependency direction, persistence, contracts, security, external integrations, deployment, runtime AI autonomy, orchestration, and technology choices.
3. Search every file in `docs/decisions/` by identifier and subject. Do not create a duplicate decision.
4. Read [references/adr-review-checklist.md](references/adr-review-checklist.md) when any architecture area is affected, an existing ADR may own the decision, or materiality is uncertain.
5. Choose exactly one action:
   - `CREATE` when a material new decision has no owning ADR.
   - `UPDATE` when an accepted ADR owns the decision and the proposal changes, replaces, or materially extends it.
   - `NOT_REQUIRED` when the work implements an accepted decision or makes a local correction without changing architecture.
   - `BLOCKED` when context is insufficient, ADR ownership is ambiguous, numbering conflicts, or the proposal contradicts repository rules without approval.
6. Cite repository evidence. Never create or edit an ADR unless the current task authorizes that write.

## Output contract

Return one and only one `ADR_ACTION` value, followed by concise evidence:

```text
ADR_ACTION: CREATE | UPDATE | NOT_REQUIRED | BLOCKED
Affected areas:
Existing ADRs:
Repository evidence:
Rationale:
Next step:
```

## Validation scenarios

- Positive trigger: bounded autonomous Catalog LLM sub-agent -> `CREATE`, or `UPDATE` if an ADR already owns it.
- Existing owner: replace the selected broker with Azure Service Bus -> `UPDATE` when a messaging ADR exists.
- Negative trigger: local null-reference fix with no boundary, contract, persistence, or runtime change -> `NOT_REQUIRED`.
- Insufficient context: “Change the architecture.” -> `BLOCKED` unless repository evidence resolves the scope.
- Unrelated formatting or one-time writing must not trigger this Skill.
