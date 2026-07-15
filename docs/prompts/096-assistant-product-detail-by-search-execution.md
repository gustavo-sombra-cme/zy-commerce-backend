# 096 - Assistant Product Detail By Search Execution

Date: 2026-07-15

## Purpose

Add a read-only assistant capability that resolves natural product detail questions by product name, SKU, or search text without requiring a product ID.

## Full Prompt

```text
APPROVED: EXECUTE Add read-only assistant product detail lookup by natural name or SKU WITH LOCAL COMMIT

Repository:
C:\ZippyYum\Learning\zy-commerce-backend

Branch:
feature/backend-assistant-product-detail-by-search

Goal:
Add a read-only assistant capability that lets users ask for product details using a natural product name, SKU, or search text instead of requiring a ProductId.

Prerequisite:
The broad catalog search branch feature/backend-assistant-broad-catalog-search must be pushed, reviewed, and merged into latest main before starting.

Start:
- Switch to main.
- Pull latest main.
- Confirm broad catalog search is merged.
- Confirm worktree is clean.
- Create branch feature/backend-assistant-product-detail-by-search.

Scope:
- Backend assistant/API layer only.
- Add CatalogGetProductBySearch intent.
- Reuse AssistantIntent.SearchText.
- Use existing SearchProductsQuery and GetProductByIdQuery.
- Keep public catalog reads active-only.
- Reuse existing response contracts.
- Add/update tests.
- Update project memory docs.
- Add prompt execution log.
- Add/update demo slide source if appropriate.
- Commit locally only after all checks pass.
- Do not push automatically.

Behavior:
- Search with SearchProductsQuery(searchText, true, 1, 2).
- Zero active matches return a supported friendly empty catalogProducts response using catalog_search and catalog-public scope.
- Exactly one active match calls GetProductByIdQuery, rechecks active state, and returns the existing catalogProduct response using catalog_search and catalog_get_product.
- Multiple active matches return up to two catalogProducts choices, ask the user to choose, do not guess, and do not call GetProductByIdQuery.

Examples:
- show me details for Galaxy
- show details for iPhone
- details for SKU ABC123
- tell me about headphones
- what is the price of Galaxy S24
- how much is iPhone

Implementation requirements:
- Add and validate CatalogGetProductBySearch with searchText only and exact catalog_search/catalog_get_product tools.
- Preserve routing precedence for safety, Orders, products-under-price, GUID detail, broad search, and unsupported fallback.
- Update deterministic routing and both LLM provider prompts.
- Handle zero, one, multiple, missing, and inactive detail cases in CatalogAssistantSubAgent using existing CQRS queries only.
- Reuse existing catalogProduct/catalogProducts structured response contracts.
- Add routing, validation, sub-agent, Text-to-SQL fallback, exposure-safety, and architecture regression tests.
- Update project memory and add assistant-product-detail-by-search demo slides.

Do not change Text-to-SQL internals, OrdersAssistantSubAgent, frontend, MCP, AssistantQueryResponse, structured response DTOs, migrations, database schema, endpoints, packages, appsettings, secrets, or write/admin assistant behavior. Do not expose inactive products, raw SQL, genericTable, or cross-user data. Do not add assistant-side ranking/fuzzy matching or direct EF/repository access. Do not push.

Verification:
- git status --short --branch
- git diff --check
- dotnet restore Ecommerce.sln
- dotnet build Ecommerce.sln --artifacts-path artifacts\product-detail-by-search-build
- dotnet test Ecommerce.sln --artifacts-path artifacts\product-detail-by-search-test
- secret scan over diff
- docs/project/CODE_REVIEW.md
- manual verification of detail, broad search, price filter, and safety prompts

Commit only when all required checks pass and CODE_REVIEW.md returns READY_TO_COMMIT. Commit message: feat: add assistant product detail by search. Stop after the local commit and return the exact next prompt APPROVED: PUSH.
```

## Status

EXECUTED

## Result Summary

Implemented `CatalogGetProductBySearch` on `feature/backend-assistant-product-detail-by-search` using the existing active-only Catalog search and product-detail CQRS queries. Zero matches return a supported empty `catalogProducts` response, one active match returns the existing `catalogProduct` response, and multiple matches return at most two choices without guessing. Deterministic and provider routing, strict plan validation, Text-to-SQL fallback coverage, regression tests, project memory, and demo slides were updated without changing Text-to-SQL internals, Orders behavior, frontend/MCP contracts, persistence, migrations, configuration, secrets, or write/admin behavior.

Verification completed on 2026-07-15:

- `dotnet restore Ecommerce.sln`: passed; only known NU1900 vulnerability-feed warnings.
- `dotnet build Ecommerce.sln --artifacts-path artifacts\product-detail-by-search-build`: passed with zero errors.
- `dotnet test Ecommerce.sln --artifacts-path artifacts\product-detail-by-search-test`: passed (Catalog 83, Auth 68, Orders 23, Architecture 209).
- Manual API verification passed for unique, zero, and multiple matches; broad catalog search and price-filter regressions; inactive-product exclusion; write/admin refusal; and raw SQL/`genericTable` non-exposure.
- `git diff --check`, scope review, secret scan, and code review passed with no blocking findings. Code-review verdict: `READY_TO_COMMIT`.
