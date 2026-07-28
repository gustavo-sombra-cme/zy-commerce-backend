# ADR Review Checklist

## Materiality

Treat a decision as material when it establishes or changes a durable constraint involving:

- Clean Architecture dependency direction or module ownership
- a bounded-context boundary or cross-module integration mechanism
- persistence models, schema strategy, migrations, or query/write-model separation
- authentication, authorization, trust boundaries, secrets, or data exposure
- public API, event, tool, or integration contracts
- external providers, packages, protocols, infrastructure, or deployment topology
- runtime AI autonomy, model/tool authority, agent orchestration, or safety limits
- a technology choice that future work must consistently follow

Local bug fixes, tests, documentation corrections, and implementation details normally do not require an ADR when they stay inside an accepted decision and create no durable constraint.

## Existing ADR ownership

1. Search `docs/decisions/` by identifier, title, affected module, technology, and decision language.
2. Prefer updating the ADR that already owns the decision.
3. Do not use a new ADR merely to contradict or silently supersede an accepted ADR.
4. Return `BLOCKED` when two ADRs appear to own the same decision or the intended supersession is unclear.

## Numbering and format

- Store ADRs in `docs/decisions/`.
- Use the next unused numeric identifier in `ADR-NNN-kebab-case-title.md` form.
- Inspect all filenames before selecting a number; never infer it from only the latest visible file.
- Include at least `Context`, `Options Considered`, `Decision`, `Rationale`, and `Consequences`.
- Follow the repository's existing `Date`, `Status`, and optional `Risks` conventions.

## Evidence checklist

Before returning an action, identify the proposed change, affected architecture areas, relevant instruction or project-memory rules, all potentially owning ADRs, and the reason the selected action is more appropriate than the other three actions.
