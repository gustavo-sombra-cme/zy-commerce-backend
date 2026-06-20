# Prompt 075: Assistant LLM Provider Integration Phase 3 Execution

## Prompt Number

075

## Date

2026-06-19

## Purpose

Execute the approved Assistant LLM Provider Integration Phase 3 plan.

## Full Prompt

APPROVED: EXECUTE Assistant LLM Provider Integration Phase 3

Scope:
- Backend only.
- Keep POST /api/assistant/query contract unchanged.
- Add provider-backed LlmAssistantIntentInterpreter behind config.
- Add Assistant:Llm options.
- Add IAssistantLlmClient.
- Use HttpClientFactory + System.Text.Json.
- Do not add SDK packages unless execution proves it is necessary and stops for approval.
- API key comes only from environment variables or user secrets.
- Do not commit secrets or config secrets.
- LLM returns structured AssistantIntentPlan only.
- Backend validates all model output through AssistantIntentPlanValidator.
- Deterministic interpreter remains fallback.
- Fail closed on disabled config, missing secret, timeout, provider failure, malformed JSON, or validation failure.
- No endpoint changes.
- No frontend changes.
- No database/migration changes.
- No live provider calls in tests.

Safety:
- Model output is untrusted.
- Reject unknown tools.
- Reject mutating/admin/SQL/cross-user plans.
- Reject model-provided buyerId/userId.
- Owner scope remains JWT sub only.
- Do not log prompts, raw responses, tokens, API keys, auth headers, or sensitive payloads.

Testing:
- Fake provider/client tests only.
- Disabled mode requires no secret.
- Fake provider maps flexible phrasing to valid plan.
- Malformed output falls back safely.
- Provider exception/timeout falls back safely.
- Unsafe requests remain unsupported.
- Architecture boundaries remain enforced.

Docs:
- Add prompt log.
- Add ADR-006 or amend ADR-005.
- Update project memory docs.

Verification:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

## Status

EXECUTED

## Result Summary

Implemented Assistant LLM Provider Integration Phase 3 as a backend-only, config-gated assistant interpreter upgrade.

Created:

- `LlmAssistantIntentInterpreter`.
- `AssistantLlmOptions`.
- `IAssistantLlmClient`.
- `HttpAssistantLlmClient` using `HttpClientFactory` and `System.Text.Json`.
- `AssistantIntentPlanJsonParser`.
- Disabled-by-default non-secret `Assistant:Llm` configuration.
- ADR-006 for config-gated LLM provider integration.

Preserved:

- Existing `POST /api/assistant/query` request/response contract.
- Deterministic interpreter as disabled-mode default and fallback.
- Existing read-only assistant capability allowlist.
- JWT `sub` owner scoping.
- Existing `AssistantIntentPlanValidator` validation before execution.

Safety:

- LLM output remains untrusted structured `AssistantIntentPlan` JSON only.
- Missing secret, disabled config, provider failure, timeout/cancellation, malformed JSON, and invalid plans fall back safely.
- No model output directly executes tools.
- No prompt, raw response, API key, token, auth header, or sensitive payload logging was added.

No provider SDK package, committed API key, committed secret, live provider call in tests, endpoint change, frontend change, Catalog/Auth/Orders module change, database change, migration, MCP dependency, write command dispatch, EF DbContext access, repository access, Domain access, raw SQL, or write handler access was added.

Verification:

- `dotnet restore Ecommerce.sln`: passed.
- `dotnet build Ecommerce.sln`: passed with 0 warnings and 0 errors.
- `dotnet test Ecommerce.sln`: passed.
- Catalog unit tests: 75 passed.
- Auth unit tests: 65 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 88 passed.
