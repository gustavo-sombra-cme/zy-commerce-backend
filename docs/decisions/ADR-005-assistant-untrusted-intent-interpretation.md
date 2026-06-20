# ADR-005: Assistant Untrusted Intent Interpretation

## Date

2026-06-19

## Status

Accepted

## Context

ADR-004 established the backend assistant as a protected API-layer orchestration boundary for read-only Catalog and Orders questions. Phase 1 used deterministic phrase routing only.

The next assistant phase needs more flexible intent interpretation while preserving the existing safety model: no assistant writes, no raw SQL, no cross-user access, no direct persistence access, no module internals, and no MCP protocol dependency.

## Options Considered

1. Let an LLM directly answer user questions.

   Rejected because assistant answers must be grounded in backend-owned data and must not expose prompts, provider responses, secrets, tokens, or hallucinated operational details.

2. Let an LLM directly choose and execute backend tools.

   Rejected because model output is untrusted. Tool names, arguments, and user scope must be validated by deterministic backend code before any execution occurs.

3. Add a real external LLM provider immediately.

   Rejected for this phase because provider package, credential, prompt governance, live network behavior, cost, timeout, and observability choices need separate approval.

4. Introduce an intent interpreter abstraction and validate structured plans before existing read-only execution.

   Accepted.

## Decision

Add `IAssistantIntentInterpreter` in the API assistant boundary.

The default implementation is `DeterministicAssistantIntentInterpreter`, which wraps the existing deterministic router and emits an `AssistantIntentPlan`.

Interpreter output is treated as untrusted. `AssistantIntentPlanValidator` validates every proposed plan before the orchestrator executes anything:

- intent kind must be known
- proposed tool names must exactly match the approved read-only capability set for the intent
- all proposed tools must exist in `AssistantToolRegistry`
- arguments must be allowed for the selected intent
- model-provided `userId`, `buyerId`, owner, subject, token, authorization, SQL, password, or connection-string arguments are rejected
- unsafe original questions are rejected even if the interpreter proposes a safe-looking plan

The assistant continues to dispatch only existing read-side Catalog and Orders CQRS queries through `ISender`.

The assistant still must not:

- dispatch write commands
- call EF Core DbContexts directly
- call repositories directly
- call Domain objects directly
- use raw SQL
- depend on MCP protocol types
- expose cross-user data
- expose tokens, authorization headers, secrets, prompts, provider details, SQL, or internal exception details

No real external LLM provider, provider package, API key, secret, configuration value, live network call, database change, migration, new endpoint, or API contract change is added in this phase.

## Rationale

This design improves language flexibility without weakening the backend execution boundary. A future LLM can only propose a structured plan; deterministic backend code remains the authority on allowed intents, tools, arguments, and authenticated user scope.

Keeping the deterministic interpreter as the default and fallback preserves current behavior and gives the system a safe path when a future interpreter fails.

## Consequences

Positive:

- Preserves the existing `POST /api/assistant/query` contract.
- Keeps assistant execution read-only and owner-scoped.
- Enables fake interpreter tests for flexible phrasing without provider packages or live network calls.
- Makes unsafe or malformed plans fail closed before `ISender` dispatch.

Tradeoffs:

- The plan validator must be updated whenever a new assistant intent or capability is explicitly approved.
- Natural-language flexibility remains limited until a real provider is separately approved.
- Exact tool-plan validation is conservative, so partially correct model plans are rejected instead of repaired.

## Risks

- Future provider integration could accidentally log sensitive prompt or response data unless logging is explicitly constrained.
- Future tool expansion could drift from the validator if allowlist changes are not tested.
- Overly broad argument names could reintroduce cross-user scope pressure unless the forbidden argument list remains conservative.
