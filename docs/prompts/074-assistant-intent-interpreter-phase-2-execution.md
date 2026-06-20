# Prompt 074: Assistant Intent Interpreter Phase 2 Execution

## Prompt Number

074

## Date

2026-06-19

## Purpose

Execute the approved Assistant Intent Interpreter Phase 2 plan.

## Full Prompt

APPROVED: EXECUTE Assistant Intent Interpreter Phase 2

Scope:
- Backend only.
- API/platform orchestration upgrade.
- No new endpoint.
- Keep POST /api/assistant/query contract unchanged.
- Add IAssistantIntentInterpreter.
- Wrap existing deterministic routing as DeterministicAssistantIntentInterpreter.
- Add AssistantIntentPlan / validation model.
- Add AssistantSafetyPolicy.
- Add AssistantIntentPlanValidator.
- Add fake/test LLM interpreter support for tests only.
- Do not add a real external LLM provider yet.
- Do not add provider packages.
- Do not add API keys, secrets, config values, or live network calls.
- Do not change Catalog, Orders, or Auth modules.
- Do not add database changes or migrations.

Rules:
- Model/interpreter output is untrusted.
- Backend must validate all proposed intents/tools/arguments.
- Reject unknown tools.
- Reject mutating/admin/SQL/cross-user plans.
- Reject model-provided userId/buyerId.
- Owner scope must still come only from JWT sub.
- Continue dispatching only existing read-side CQRS queries through ISender.
- Do not dispatch write commands.
- Do not call EF DbContexts, repositories, Domain objects, SQL, MCP protocol types, or write handlers directly.

Testing:
- Deterministic interpreter is default.
- Fake LLM interpreter can map flexible phrasing to structured plans in tests.
- Invalid model plans fail closed.
- Provider/interpreter failure falls back safely.
- Unsafe requests remain unsupported.
- Owner scoping still uses JWT sub.
- Architecture tests confirm assistant does not reference EF, repositories, Domain internals, MCP protocol types, or write commands.

Documentation:
- Add prompt log.
- Amend ADR-004 or add ADR-005 for untrusted intent interpretation.
- Update project memory docs.

Verification:
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln
- dotnet test Ecommerce.sln

## Status

EXECUTED

## Result Summary

Implemented Assistant Intent Interpreter Phase 2 as an API/platform-only assistant orchestration upgrade.

Created:

- `IAssistantIntentInterpreter`.
- `DeterministicAssistantIntentInterpreter` as the production default interpreter and fallback.
- `AssistantIntentPlan`, `AssistantSafetyPolicy`, and `AssistantIntentPlanValidator`.
- Strict untrusted-plan validation for intent kind, exact read-only tool plan, allowed arguments, unsafe questions, unknown tools, and model-provided user/buyer scope.
- Fake interpreter test support in architecture tests only.
- ADR-005 for untrusted assistant intent interpretation.
- Project memory updates.

Preserved:

- Existing `POST /api/assistant/query` request/response contract.
- JWT `sub` owner scoping for Orders analysis.
- Existing read-side CQRS dispatch through `ISender`.
- Existing assistant capability allowlist.

No real external LLM provider, provider package, API key, secret, configuration value, live network call, endpoint, Catalog/Auth/Orders module change, database change, migration, MCP dependency, write command dispatch, raw SQL, EF DbContext access, repository access, Domain access, or write handler access was added.

Verification:

- `dotnet restore Ecommerce.sln`: passed.
- `dotnet build Ecommerce.sln`: passed.
- `dotnet test Ecommerce.sln`: passed.
- Catalog unit tests: 75 passed.
- Auth unit tests: 65 passed.
- Orders unit tests: 23 passed.
- Architecture tests: 81 passed.
