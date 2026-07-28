# Code References

Last updated: 2026-06-19

## Backend

### API Layer

- Program and middleware registration: `src/Api/Ecommerce.Api/Program.cs`
- Catalog controller: `src/Api/Ecommerce.Api/Controllers/Catalog/ProductsController.cs`
- Auth controller: `src/Api/Ecommerce.Api/Controllers/Auth/AuthUsersController.cs`
- Orders controller: `src/Api/Ecommerce.Api/Controllers/Orders/OrdersController.cs`
- MCP tools: `src/Api/Ecommerce.Api/Mcp/EcommerceMcpTools.cs`
- Current MCP user helper: `src/Api/Ecommerce.Api/Mcp/CurrentUser.cs`
- Assistant controller: `src/Api/Ecommerce.Api/Controllers/Assistant/AssistantController.cs`
- Assistant orchestration: `src/Api/Ecommerce.Api/Assistant/AssistantOrchestrator.cs`
- Assistant routing and allowlist: `src/Api/Ecommerce.Api/Assistant/AssistantIntentRouter.cs`, `AssistantToolRegistry.cs`, `AssistantToolNames.cs`
- Assistant contracts: `src/Api/Ecommerce.Api/Assistant/AssistantQueryRequest.cs`, `AssistantQueryResponse.cs`
- Correlation ID middleware: `src/Api/Ecommerce.Api/Middleware/CorrelationIdMiddleware.cs`
- Request logging: `src/Api/Ecommerce.Api/Middleware/RequestLoggingMiddleware.cs`
- Exception handling: `src/Api/Ecommerce.Api/Middleware/ExceptionHandlingMiddleware.cs`
- Database health check: `src/Api/Ecommerce.Api/HealthChecks/DbContextHealthCheck.cs`

### Catalog Module

- Aggregate: `src/Modules/Catalog/Ecommerce.Catalog.Domain/Products/Product.cs`
- Value objects: `ProductId.cs`, `Sku.cs`, `ProductName.cs`
- Create product CQRS: `src/Modules/Catalog/Ecommerce.Catalog.Application/Products/CreateProduct`
- Search products CQRS: `src/Modules/Catalog/Ecommerce.Catalog.Application/Products/SearchProducts`
- Get product CQRS: `src/Modules/Catalog/Ecommerce.Catalog.Application/Products/GetProductById`
- Update details CQRS: `src/Modules/Catalog/Ecommerce.Catalog.Application/Products/UpdateProductDetails`
- Deactivate/reactivate CQRS: `DeactivateProduct`, `ReactivateProduct`
- Contracts: `src/Modules/Catalog/Ecommerce.Catalog.Contracts/Products`
- Persistence: `CatalogDbContext.cs`, `CatalogReadDbContext.cs`, `ProductConfiguration.cs`
- Read model: `ProductSearchReadModel.cs`
- Migrations: `20260608111338_InitialCatalogSchema.cs`, `20260618090000_AddProductPrice.cs`

### Auth Module

- Aggregate and value objects: `src/Modules/Auth/Ecommerce.Auth.Domain/Users`
- Register CQRS: `src/Modules/Auth/Ecommerce.Auth.Application/Users/RegisterUser`
- Login CQRS: `src/Modules/Auth/Ecommerce.Auth.Application/Users/LoginUser`
- Security abstractions: `src/Modules/Auth/Ecommerce.Auth.Application/Security`
- JWT generator: `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Security/JwtAccessTokenGenerator.cs`
- Password hasher: `src/Modules/Auth/Ecommerce.Auth.Infrastructure/Security/PasswordHasher.cs`
- Persistence: `AuthDbContext.cs`, `UserConfiguration.cs`, `UserRepository.cs`
- Contracts: `src/Modules/Auth/Ecommerce.Auth.Contracts/Users`

### Orders Module

- Aggregate: `src/Modules/Orders/Ecommerce.Orders.Domain/Orders/Order.cs`
- Child entity: `OrderLine.cs`
- Value objects: `OrderId.cs`, `OrderLineId.cs`, `BuyerId.cs`
- Create order CQRS: `src/Modules/Orders/Ecommerce.Orders.Application/Orders/CreateOrder`
- List orders CQRS: `src/Modules/Orders/Ecommerce.Orders.Application/Orders/ListOrdersForBuyer`
- Get order CQRS: `src/Modules/Orders/Ecommerce.Orders.Application/Orders/GetOrderById`
- Contracts: `src/Modules/Orders/Ecommerce.Orders.Contracts/Orders`
- Persistence: `OrdersDbContext.cs`, `OrderConfiguration.cs`, `OrderLineConfiguration.cs`, `OrderReadRepository.cs`, `OrderRepository.cs`

### Backend Tests

- Architecture tests: `tests/ArchitectureTests/Ecommerce.ArchitectureTests`
- Assistant architecture/integration tests: `tests/ArchitectureTests/Ecommerce.ArchitectureTests/AssistantIntegrationTests.cs`
- Bounded Catalog agent tests: `tests/ArchitectureTests/Ecommerce.ArchitectureTests/CatalogAutonomousAgentTests.cs`
- Assistant authorization tests: `tests/ArchitectureTests/Ecommerce.ArchitectureTests/ApiAuthorizationTests.cs`
- Catalog unit tests: `tests/UnitTests/Ecommerce.Catalog.UnitTests/Products`
- Auth unit tests: `tests/UnitTests/Ecommerce.Auth.UnitTests/Users`
- Orders unit tests: `tests/UnitTests/Ecommerce.Orders.UnitTests/Orders`

## Frontend

### App and Routing

- Root app: `../zy-commerce-frontend/src/app/app.component.ts`
- App config: `../zy-commerce-frontend/src/app/app.config.ts`
- Root routes: `../zy-commerce-frontend/src/app/app.routes.ts`
- Auth routes: `features/auth/auth.routes.ts`
- Catalog routes: `features/catalog/catalog.routes.ts`
- Product details routes: `features/catalog/product-details.routes.ts`
- Cart routes: `features/cart/cart.routes.ts`
- Orders routes: `features/orders/orders.routes.ts`
- MCP assistant routes: `features/mcp-assistant/mcp-assistant.routes.ts`

### Core Frontend

- Auth client: `core/auth/auth-api.client.ts`
- Auth session: `core/auth/auth-session.service.ts`
- Auth guard: `core/auth/auth.guard.ts`
- Token storage abstraction: `core/auth/token-storage.provider.ts`, `session-token-storage.service.ts`
- Runtime config: `core/config/runtime-config.service.ts`, `runtime-config.model.ts`
- Authorization interceptor: `core/http/authorization.interceptor.ts`
- Correlation ID interceptor/service: `core/http/correlation-id.interceptor.ts`, `correlation-id.service.ts`
- App shell: `core/layout/app-shell`

### Frontend Features

- Catalog client/models: `features/catalog/data/catalog-api.client.ts`, `catalog.models.ts`
- Catalog page: `features/catalog/catalog-page`
- Product details page: `features/catalog/product-details-page`
- Cart state/models: `features/cart/data/cart-state.service.ts`, `cart.models.ts`
- Cart page: `features/cart/cart-page`
- Orders client/models: `features/orders/data/orders-api.client.ts`, `orders.models.ts`
- Checkout mapper: `features/orders/data/create-order.mapper.ts`
- Checkout page: `features/orders/checkout-page`
- Orders list/details pages: `features/orders/orders-page`, `order-details-page`
- MCP registry/client/confirmation: `src/app/mcp`
- MCP assistant page: `features/mcp-assistant/mcp-assistant-page`

### Frontend Tests

- App shell and root tests: `src/app/*.spec.ts`, `core/layout/**/*.spec.ts`
- Auth tests: `core/auth/**/*.spec.ts`, `features/auth/**/*.spec.ts`
- Catalog tests: `features/catalog/**/*.spec.ts`
- Cart tests: `features/cart/**/*.spec.ts`
- Orders tests: `features/orders/**/*.spec.ts`
- MCP tests: `src/app/mcp/**/*.spec.ts`, `features/mcp-assistant/**/*.spec.ts`

## Documentation References

- Backend project memory: `docs/project`
- Backend ADRs: `docs/decisions`
- Backend prompt logs: `docs/prompts`
- Frontend project docs: `../zy-commerce-frontend/docs/project`
- Frontend agent rules: `../zy-commerce-frontend/AGENT.md`
