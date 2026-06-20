# ADR-006: Assistant LLM Provider Integration

## Date

2026-06-19

## Status

Accepted

## Context

ADR-004 established the backend assistant as a protected API-layer, read-only orchestration boundary. ADR-005 added untrusted `AssistantIntentPlan` interpretation and deterministic validation before execution.

Phase 3 adds a real provider-backed interpreter behind configuration while preserving the same safety boundary. The provider must improve natural-language interpretation only; it must not answer users directly or execute backend capabilities.

## Options Considered

1. Add a provider SDK package.

   Rejected for this phase because `HttpClientFactory` and `System.Text.Json` are sufficient for a narrow structured-plan adapter, and avoiding SDK packages keeps dependency impact smaller.

2. Use provider tool/function calling directly.

   Rejected because model-selected tool execution would weaken the existing backend allowlist and validation boundary.

3. Store the API key in appsettings.

   Rejected because secrets must come only from environment variables or user secrets and must not be committed.

4. Add a provider-backed `LlmAssistantIntentInterpreter` that returns only structured `AssistantIntentPlan` data.

   Accepted.

## Decision

Add `LlmAssistantIntentInterpreter` behind `Assistant:Llm` configuration.

Non-secret defaults may be committed, but provider secrets must not be committed. When enabled without a resolved API key, the provider call is skipped and deterministic fallback remains active. The API key is resolved at runtime from:

1. the configured environment variable name
2. user secrets or other non-committed configuration providers bound to `Assistant:Llm:ApiKey`

The provider adapter uses:

- `IHttpClientFactory`
- `System.Text.Json`
- the OpenAI Responses API request shape with `model`, `input`, and structured `text.format`
- a strict parser for structured plan JSON
- a timeout from `Assistant:Llm:TimeoutSeconds`
- deterministic fallback when disabled, unconfigured, missing a secret, timed out, failed, or malformed

The model returns only a structured JSON object shaped like `AssistantIntentPlan`:

- `kind`
- `tools`
- `arguments`

The backend still validates all model output through `AssistantIntentPlanValidator` before any assistant execution.

The provider integration must not:

- execute tools directly
- dispatch write commands
- call EF Core DbContexts directly
- call repositories directly
- call Domain objects directly
- use raw SQL
- depend on MCP protocol types
- expose or log prompts, raw responses, API keys, tokens, auth headers, or sensitive payloads
- change `POST /api/assistant/query`
- add frontend behavior
- add database changes or migrations

## Rationale

This keeps the LLM in a narrow interpretation role. The backend remains the authority for allowed intents, tools, arguments, and authenticated user scope.

Using `HttpClientFactory` keeps the adapter explicit, testable, and free of provider SDK dependency churn. Fake `IAssistantLlmClient` tests cover provider success, malformed output, timeout/failure, disabled mode, and unsafe requests without live network calls.

## Consequences

Positive:

- Flexible phrasing can be interpreted by a real provider when explicitly enabled.
- Disabled mode requires no secret and preserves deterministic behavior.
- Provider failures fall back safely without exposing operational details.
- The existing public assistant API fields remain unchanged; additive optional structured response fields may be ignored by existing clients.

Tradeoffs:

- The HTTP payload is provider-specific and may need adjustment if a different provider is introduced.
- The strict JSON parser rejects non-JSON, markdown-wrapped JSON, or schema drift instead of attempting repair.
- Provider observability is intentionally limited because prompts and raw responses must not be logged.

## Risks

- Provider misconfiguration may silently fall back to deterministic interpretation.
- Future changes could accidentally log prompts or responses unless the no-sensitive-logging rule remains enforced.
- A future provider-specific feature could tempt direct tool execution, which must remain rejected unless a new architecture decision supersedes this one.
