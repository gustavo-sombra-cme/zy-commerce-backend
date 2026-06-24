# Prompt 082: Gemini LLM Provider Planning

## Date

2026-06-24

## Purpose

Plan adding Gemini as a selectable external intent-interpretation provider for the existing read-only Ecommerce Assistant.

## Full Prompt

Plan backend feature: Gemini LLM Provider for Ecommerce Assistant.

Add Gemini as a selectable LLM provider for the existing Ecommerce Assistant so the assistant can be tested with Gemini API free-tier usage for POC/demo purposes. Keep the assistant provider-based, preserve deterministic fallback and existing OpenAI-style provider behavior, keep assistant output untrusted, keep tools read-only, do not expose admin tools, do not change frontend or MCP, and do not change assistant response contracts unless required.

Use environment/config support for:

- `ECOMMERCE_ASSISTANT_LLM_PROVIDER=Gemini`
- `ECOMMERCE_ASSISTANT_GEMINI_API_KEY=<secret>`
- `ECOMMERCE_ASSISTANT_GEMINI_MODEL=gemini-2.5-flash`
- `ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT=https://generativelanguage.googleapis.com/v1beta`

Use Gemini Developer API `generateContent`, parse candidate text as structured assistant intent JSON, validate through the existing backend validator, and fall back safely on failures.

## Status

PLANNED

## Result Summary

Planned a backend-only API-layer provider addition that keeps Gemini in a narrow intent-planning role. The plan preserves deterministic fallback, existing OpenAI-style provider behavior, untrusted model-output validation, read-only assistant capabilities, no frontend changes, no MCP changes, no database changes, no migrations, no committed secrets, and no prompt/raw response/key logging.
