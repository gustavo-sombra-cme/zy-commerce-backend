# Prompt 065 - Orders Initial Vertical Slice Planning

## Prompt Number

065

## Date

2026-06-12

## Purpose

Plan the Orders Initial Vertical Slice as a new Domain Feature module.

## Full Prompt

Plan new module: Orders - Initial Vertical Slice

Goal:
Introduce an Orders module before MCP integration.

Use docs/project/PROMPT_TEMPLATE.md exactly.
Classify this as a Domain Feature.

Scope:
- Create Order
- Get Order By Id

Focus on:
- Clean Architecture module boundaries
- DDD aggregate design
- Order aggregate
- OrderLine design
- Product snapshot strategy
- Authorization requirements
- CQRS design
- Persistence design
- Testing strategy
- Future MCP exposure

Out of Scope:
- Payments
- Inventory reservation
- Shipping
- Discounts
- Coupons
- Order cancellation
- Refunds
- Advanced order status workflows
- Customer profile module

PLAN_STATUS: PENDING_APPROVAL

## Status

PLANNED

## Result Summary

Planned a new Orders module with Domain, Application, Infrastructure, Contracts, and UnitTests projects. The plan covers Create Order and Get Order By Id only, uses product snapshot data in the create request, keeps Orders isolated from Catalog and Auth internals, requires bearer authentication for both endpoints, and identifies an ADR for the product snapshot strategy.
