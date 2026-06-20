# Prompt 067 - MCP Server Integration Planning

## Prompt Number

067

## Date

2026-06-12

## Purpose

Plan the MCP Server Integration as an API/platform improvement.

## Full Prompt

Plan platform feature: MCP Server Integration

Goal:
Expose selected e-commerce capabilities through an MCP server after completing Catalog, Auth, Orders, health checks, and structured logging.

Use docs/project/PROMPT_TEMPLATE.md exactly.
Classify this as an API / Platform Improvement.

Focus on:
- MCP server boundary
- Whether MCP should call API endpoints or Application layer
- Authentication and user context
- Tool/resource design
- Catalog tools
- Orders tools
- What must not be exposed
- Security risks
- Frontend integration through MCP
- Phased implementation

PLAN_STATUS: PENDING_APPROVAL

## Status

PLANNED

## Result Summary

Planned MCP Server Integration as an API/platform-only adapter hosted in `Ecommerce.Api`. The plan chose an allowlisted MCP boundary that dispatches existing Application/CQRS requests through `ISender`, keeps module internals isolated, protects `/mcp` with bearer authentication, exposes only initial Catalog and Orders tools, requires explicit confirmation for order creation, and calls for ADR-003 to document MCP boundary and security.
