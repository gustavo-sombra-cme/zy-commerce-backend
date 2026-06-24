# Prompt 083: Gemini LLM Provider Execution

## Date

2026-06-24

## Purpose

Execute the approved backend Gemini provider feature with verification, code review, and a local commit only if all gates pass.

## Full Prompt

APPROVED: EXECUTE Backend Gemini LLM Provider WITH LOCAL COMMIT.

Add Gemini as a selectable external intent-interpretation provider for the existing Ecommerce Assistant. Keep the existing assistant architecture: `POST /api/assistant/query` -> `AssistantOrchestrator` -> `IAssistantIntentInterpreter` -> `LlmAssistantIntentInterpreter` -> configured `IAssistantLlmClient` -> untrusted `AssistantIntentPlan` JSON -> parser -> validator -> existing read-only Catalog/Orders CQRS queries through `ISender` -> existing `AssistantQueryResponse`.

Scope is backend only. Do not change frontend, MCP, database, migrations, Domain/Application/Infrastructure behavior outside the assistant/API provider boundary, assistant response contract, admin tools, mutating tools, or secrets. Preserve deterministic fallback and existing OpenAI-style provider behavior. Add `GeminiAssistantLlmClient` using `HttpClientFactory` and `System.Text.Json`. Add provider selection in DI. Support Gemini environment/config values. Use REST `generateContent`, send only planning instructions plus user question, parse candidate text, reject malformed output, and fall back safely. Do not log API keys, auth headers, JWTs, raw prompts, raw responses, sensitive user/order data, or full Gemini request URIs.

Run restore, build, test, and `docs/project/CODE_REVIEW.md`. Commit locally with `feat: add Gemini assistant provider` only when verification and review pass. Stop before push.

## Status

EXECUTED

## Result Summary

Implemented Gemini as a selectable backend assistant intent provider with REST `generateContent` request handling, safe candidate text extraction, provider selection in DI, deterministic fallback preservation, and tests for request shape, parsing, malformed responses, missing config, provider selection, safe logging, and existing assistant safety boundaries. Updated project memory and API reference documentation. Verification passed with `dotnet restore Ecommerce.sln`, `dotnet build Ecommerce.sln`, and `dotnet test Ecommerce.sln`.
