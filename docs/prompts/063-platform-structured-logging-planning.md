# Prompt 063 - Platform Structured Logging Planning

## Prompt Number

063

## Date

2026-06-12

## Purpose

Plan the Platform Structured Logging improvement as an API/platform-only feature.

## Full Prompt

Plan platform feature: Structured Logging

Additional Requirements:
- Use built-in ASP.NET Core logging only.
- Add X-Correlation-ID support.
- Return X-Correlation-ID on every response.
- Preserve incoming X-Correlation-ID when supplied.
- Do not log tokens, authorization headers, passwords, request bodies, or response bodies.
- Keep this API/platform-only.
- Do not change Domain, Application, CQRS, database schema, modules, packages, or business behavior.

## Status

PLANNED

## Result Summary

Planned an API/platform-only structured logging design using built-in ASP.NET Core logging. The plan covers correlation ID middleware, request logging middleware, safe exception logging, health-check readiness logging, and authentication-related logging without packages, migrations, Domain, Application, CQRS, module, database, or business behavior changes.
