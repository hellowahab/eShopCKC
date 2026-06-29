# eShopCKC — Monolith to Microservices Migration Plan

> **Scope**: Planning only. No code or implementation is included.
> **Target Architecture**: Event-driven microservices with an API Gateway, using .NET 9 ASP.NET Core Minimal APIs.

---

## Part 1 — Current State Analysis

### 1.1 Solution at a Glance

| Layer | Project | Role |
|---|---|---|
| Domain | `Ckc.EShop.ApplicationCore` | Entities, Interfaces, Specifications |
| Data | `Ckc.EShop.Infrastructure` | EF Core Contexts, Repositories, Identity |
| Presentation | `ckc.eShop.Web` | MVC Controllers, Services, Razor Views |
| Tests | `Ckc.EShop.ApplicationCore.Tests` | xUnit entity tests |

### 1.2 Existing DbContexts (Bounded Contexts Already Visible)

The infrastructure layer already has **separate DbContexts per domain** — a very strong signal that the bounded contexts are well understood:

| DbContext | Entities Owned | Current DB |
|---|---|---|
| `CatalogDbContext` | `CatalogItem`, `CatalogBrand`, `CatalogType` | InMemory |
| `BasketDbContext` | `Basket`, `BasketItems` | InMemory |
| `OrderDbContext` | `Order`, `OrderItem`, `Address`, `CatalogItemOrdered` | InMemory |
| `AppIdentityDbContext` | `ApplicationUser`, Roles | InMemory |

### 1.3 Current Service Inventory

| Service Class | Interfaces Used | Domain(s) Touched |
|---|---|---|
| `CatalogService` | `IRepository<CatalogItem>`, `IRepository<CatalogBrand>`, `IRepository<CatalogType>` | Catalog only ✅ |
| `BasketService` | `IAsyncRepository<Basket>`, **`IRepository<CatalogItem>`** | Basket + **Catalog ⚠️** |
| `OrderService` | `IAsyncRepository<Order>`, `IAsyncRepository<Basket>`, **`IAsyncRepository<CatalogItem>`** | Order + **Basket + Catalog ⚠️** |

### 1.4 Cross-Domain Coupling Problems (The Couplings to Break)

These are the exact lines in the monolith where domain boundaries are violated:

#### 🔴 Problem 1 — `BasketService` reads `CatalogItem` directly
```
// BasketService.cs:102
var item = _itemRepository.GetById(i.CatalogItemId);
itemModel.PictureUrl = _uriComposer.ComposePicUri(item.PictureUri);
itemModel.ProductName = item.Name;
```
**Why it's a problem**: The Basket service queries the Catalog database directly to enrich basket items with product names and images. In microservices, each service owns its own data — this must become an HTTP call or a cached denormalized copy.

#### 🔴 Problem 2 — `OrderService` reads both `Basket` and `CatalogItem`
```
// OrderService.cs:28-38
var basket = await _basketRepository.GetByIdAsync(basketId);
foreach (var item in basket.Items) {
    var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
    ...
}
```
**Why it's a problem**: Order creation requires data from two other domains. This must become an event-driven flow — Basket publishes a `CheckoutInitiated` event, Order Service subscribes and builds the order autonomously.

#### 🔴 Problem 3 — `BasketController` calls `IOrderService` directly
```
// BasketController.cs:53-54
await _orderService.CreateOrderAsync(basketViewModel.Id,
    new Address("street", "city", "state", "country", "1234"));
```
**Why it's a problem**: The Web layer is orchestrating an Order operation from within the Basket controller. This is a tight synchronous coupling that must be replaced with an event or an API Gateway aggregation.

#### 🔴 Problem 4 — `AccountController` calls `IBasketService` on login
```
// AccountController.cs:58
await _basketService.TransferBasketAsync(anonymousId, model.Email);
```
**Why it's a problem**: The Identity/Auth service mutates Basket state directly. In microservices, a `UserLoggedIn` event should trigger the Basket service to do the transfer independently.

#### 🟡 Warning — `TransferBasketAsync` is not implemented
```
// BasketService.cs:143-145
public Task TransferBasketAsync(string anonymousId, string userName)
{
    throw new NotImplementedException();
}
```
This unfinished method is a risk that must be resolved before or during the migration.

---

## Part 2 — Target Architecture

### 2.1 Microservices to Create

| # | Microservice | Responsibility | Own Database | Technology |
|---|---|---|---|---|
| 1 | **Catalog API** | Products, brands, types, images | SQL Server / PostgreSQL | ASP.NET Core Minimal API |
| 2 | **Basket API** | Cart lifecycle (add/remove/clear) | **Redis** (in-memory, volatile) | ASP.NET Core Minimal API |
| 3 | **Order API** | Order creation, order history, order status | SQL Server / PostgreSQL | ASP.NET Core Minimal API |
| 4 | **Identity API** | Registration, login, JWT token issuance | SQL Server (Identity tables) | ASP.NET Core + Identity Server / Duende |
| 5 | **API Gateway** | Single entry point, routing, auth validation | — | YARP (Yet Another Reverse Proxy) |
| 6 | **Web BFF** | Aggregates data from APIs for the Razor UI | — | ASP.NET Core MVC (existing, refactored) |

### 2.2 Communication Patterns

| Type | When to Use | Technology |
|---|---|---|
| **Synchronous HTTP/REST** | Query operations (read data for display) | `HttpClient` with Polly resilience |
| **Asynchronous Events** | State-changing cross-domain operations | RabbitMQ or Azure Service Bus |
| **gRPC (optional)** | High-frequency internal service calls | Protobuf contracts |

### 2.3 Event Catalog (Events to Publish/Subscribe)

| Event | Publisher                         | Subscriber(s)         | Purpose |
|---|---|---|---|
| `BasketCheckoutInitiated` | Basket API    | Order API             | Trigger order creation from basket contents |
| `OrderCreated`            | Order API     | Basket API            | Confirm order placed → clear the basket |
| `OrderStatusChanged`      | Order API     | Notification (future) | Inform user of status updates |
| `UserLoggedIn`            | Identity API  | Basket API            | Transfer anonymous basket to authenticated user |
| `CatalogItemPriceChanged` | Catalog API   | Basket API            | Update stale prices in active baskets |

### 2.4 High-Level Architecture Diagram

```
┌──────────────────────────────────────────────────────────────────┐
│                         Browser / Client                         │
└────────────────────────────┬─────────────────────────────────────┘
                             │ HTTPS
┌────────────────────────────▼─────────────────────────────────────┐
│                     API Gateway (YARP)                           │
│   - Route /catalog/** → Catalog API                              │
│   - Route /basket/**  → Basket API                               │
│   - Route /orders/**  → Order API                                │
│   - Route /identity/**→ Identity API                             │
│   - Validate JWT on all secured routes                           │
└──┬──────────┬──────────┬──────────┬────────────────────────────-─┘
   │          │          │          │
   ▼          ▼          ▼          ▼
┌──────┐  ┌──────┐  ┌──────┐  ┌──────────┐
│Catalog│  │Basket│  │Order │  │Identity  │
│  API  │  │  API │  │  API │  │   API    │
└──┬───┘  └──┬───┘  └──┬───┘  └────┬─────┘
   │          │          │          │
   ▼          ▼          ▼          ▼
┌──────┐  ┌──────┐  ┌──────┐  ┌──────────┐
│Catalog│  │Redis │  │Order │  │Identity  │
│  DB   │  │Cache │  │  DB  │  │   DB     │
└───────┘  └──────┘  └──────┘  └──────────┘
           
           ┌───────────────────────────────┐
           │     Message Broker            │
           │  (RabbitMQ / Azure Svc Bus)   │
           └───────────────────────────────┘
```

---

## Part 3 — Bounded Context Design

### 3.1 Catalog Service Bounded Context

**Owns:**
- `CatalogItem` (Id, Name, Description, Price, PictureUri, CatalogBrandId, CatalogTypeId)
- `CatalogBrand` (Id, Brand)
- `CatalogType` (Id, Type)

**API Endpoints to Expose:**
- `GET /api/catalog/items?page&pageSize&brand&type` — paginated product list
- `GET /api/catalog/items/{id}` — single product detail
- `GET /api/catalog/brands` — all brands
- `GET /api/catalog/types` — all types
- `GET /api/catalog/pics/{id}` — product image (move from Web layer)

**Publishes:** `CatalogItemPriceChanged`

**Database:** Dedicated SQL Server database, seeded from `CatalogContextSeed`

---

### 3.2 Basket Service Bounded Context

**Owns:**
- `Basket` (Id, BuyerId)
- `BasketItem` (Id, CatalogItemId, **ProductName**, **PictureUri**, UnitPrice, Quantity)

> ⚠️ **Key Design Decision**: `ProductName` and `PictureUri` must be **denormalized** into `BasketItem`. When a user adds to cart, the Basket service calls `GET /api/catalog/items/{id}` synchronously and stores the snapshot. This removes the cross-domain read dependency at display time.

**API Endpoints to Expose:**
- `GET /api/basket/{buyerId}` — get or create basket
- `POST /api/basket` — update basket (add/remove/update qty)
- `DELETE /api/basket/{buyerId}` — delete basket
- `POST /api/basket/checkout` — publish `BasketCheckoutInitiated` event

**Subscribes to:** `OrderCreated` (to delete basket after order confirmed), `UserLoggedIn` (to transfer basket), `CatalogItemPriceChanged` (to update stale prices)

**Database:** **Redis** — basket data is inherently volatile and session-like; Redis is the natural fit

---

### 3.3 Order Service Bounded Context

**Owns:**
- `Order` (Id, BuyerId, OrderDate, ShipToAddress, Status)
- `OrderItem` (Id, CatalogItemId, ProductName, PictureUri, UnitPrice, Units)
- `Address` (value object)
- `CatalogItemOrdered` (snapshot at time of order)

**API Endpoints to Expose:**
- `GET /api/orders?buyerId` — list orders for a user
- `GET /api/orders/{orderId}` — order detail
- `PUT /api/orders/{orderId}/status` — update order status (admin)

**Subscribes to:** `BasketCheckoutInitiated` (to create the order asynchronously)

**Publishes:** `OrderCreated`, `OrderStatusChanged`

**Database:** Dedicated SQL Server database

---

### 3.4 Identity Service Bounded Context

**Owns:**
- `ApplicationUser` (Id, UserName, Email, PasswordHash, Roles)

**API Endpoints to Expose:**
- `POST /api/identity/register` — user registration
- `POST /api/identity/login` — authenticate, return JWT + refresh token
- `POST /api/identity/refresh` — refresh access token
- `POST /api/identity/logout` — invalidate refresh token

**Publishes:** `UserLoggedIn`

**Key Change**: Replace cookie-based ASP.NET Identity auth with **JWT Bearer tokens**. All other services validate the token against the Identity Service's public key (or a shared secret). No service calls back to Identity for every request.

---

## Part 4 — Migration Strategy: The Strangler Fig Pattern

Rather than a big-bang rewrite, use the **Strangler Fig Pattern** — incrementally extract services while keeping the monolith running, routing traffic through the new API Gateway progressively.

### Phase 0 — Foundation (Week 1–2)

**Goal**: Set up shared infrastructure before touching any business logic.

| Task | Detail |
|---|---|
| Create new solution `eShopCKC.Microservices.sln` | Keep the monolith solution intact and untouched |
| Set up API Gateway project | Add YARP NuGet, configure basic routing passthrough to existing monolith |
| Set up Message Broker | Install RabbitMQ locally via Docker (`docker run rabbitmq`) or provision Azure Service Bus |
| Create shared NuGet packages | `eShopCKC.Events` — shared event contracts (DTOs only, no logic) |
| Set up Docker Compose | One file to spin up all infrastructure (RabbitMQ, Redis, SQL Server instances) |
| Set up GitHub Actions CI | Build + test pipeline for the new solution |

**Deliverable**: A running API Gateway that proxies 100% of traffic to the existing monolith. Users notice no change.

---

### Phase 1 — Extract Identity Service (Week 3–4)

**Why first?** Every other service needs auth. Getting JWT working first unlocks everything else.

| Task | Detail |
|---|---|
| Create `Identity.API` project | New ASP.NET Core Minimal API project |
| Migrate `AppIdentityDbContext` | Move to its own SQL Server database |
| Migrate `ApplicationUser`, `AppIdentityDbContextSeed` | Copy entity and seed data |
| Implement JWT issuance | Replace `SignInManager` cookie auth with `JwtSecurityTokenHandler` |
| Implement refresh token | Store refresh tokens in Identity DB |
| Expose `/api/identity/register`, `/api/identity/login` | New endpoints replacing `AccountController` |
| Configure API Gateway | Route `/identity/**` to `Identity.API` |
| Configure JWT validation in API Gateway | YARP validates JWT; propagates `sub` claim to downstream services |
| Publish `UserLoggedIn` event | From Identity API via RabbitMQ |

**Testing Gate**: Log in via the new Identity API, receive a JWT, call a protected existing monolith endpoint using the JWT — it should work.

---

### Phase 2 — Extract Catalog Service (Week 5–6)

**Why second?** Catalog is the least coupled service (no cross-domain writes, read-only for others). Lowest risk.

| Task | Detail |
|---|---|
| Create `Catalog.API` project | New ASP.NET Core Minimal API project |
| Migrate `CatalogDbContext` | Move to its own SQL Server database |
| Migrate entities | `CatalogItem`, `CatalogBrand`, `CatalogType`, `CatalogSettings` |
| Migrate `CatalogContextSeed` | Seed data moves with the service |
| Implement REST endpoints | `GET /items`, `GET /items/{id}`, `GET /brands`, `GET /types`, `GET /pics/{id}` |
| Implement `CatalogItemPriceChanged` event publisher | When admin updates price, publish event to broker |
| Configure API Gateway routing | Route `/catalog/**` to `Catalog.API` |
| Update `CatalogController` in monolith | Replace direct `IRepository` calls with `HttpClient` calls to `Catalog.API` |

**Testing Gate**: Catalog page loads from the new Catalog.API. Image serving works. Filtering by brand/type works.

---

### Phase 3 — Extract Basket Service (Week 7–9)

**Why third?** Basket depends on Catalog (for product enrichment) but is independent of Orders at read time. Phase 2 (Catalog API) must be done first.

| Task | Detail |
|---|---|
| Create `Basket.API` project | New ASP.NET Core Minimal API project |
| Swap `BasketDbContext` for Redis | Use `StackExchange.Redis` or `Microsoft.Extensions.Caching.StackExchangeRedis` |
| Redesign `BasketItem` to include denormalized fields | Add `ProductName` and `PictureUri` to `BasketItem` (stored in Redis) |
| Implement REST endpoints | `GET /basket/{buyerId}`, `POST /basket`, `DELETE /basket/{buyerId}` |
| Implement `POST /basket/checkout` | Publish `BasketCheckoutInitiated` event to message broker |
| Subscribe to `OrderCreated` event | Delete basket when order is confirmed |
| Subscribe to `UserLoggedIn` event | Implement the `TransferBasketAsync` logic (currently `NotImplementedException`) |
| Subscribe to `CatalogItemPriceChanged` | Scan Redis baskets and update stale prices |
| Configure API Gateway routing | Route `/basket/**` to `Basket.API` |
| Update `BasketController` in monolith | Replace `IBasketService` calls with `HttpClient` |
| **Remove** `IOrderService` from `BasketController` | Checkout now publishes event instead of calling `OrderService` directly |

**Testing Gate**: Add items to cart, checkout publishes the event (verify in RabbitMQ management UI), basket is stored in Redis.

---

### Phase 4 — Extract Order Service (Week 10–12)

**Why last?** Order creation now depends on the `BasketCheckoutInitiated` event from Phase 3. Must be done after Basket is extracted.

| Task | Detail |
|---|---|
| Create `Order.API` project | New ASP.NET Core Minimal API project |
| Migrate `OrderDbContext` | Move to its own SQL Server database |
| Migrate entities | `Order`, `OrderItem`, `Address`, `CatalogItemOrdered` |
| Implement REST endpoints | `GET /orders?buyerId`, `GET /orders/{id}`, `PUT /orders/{id}/status` |
| Subscribe to `BasketCheckoutInitiated` | Build the `OrderItem` list from the event payload (no DB call needed) |
| Publish `OrderCreated` event | After persisting the order |
| Configure API Gateway routing | Route `/orders/**` to `Order.API` |
| Update `OrderController` in monolith | Replace `IOrderRepository` calls with `HttpClient` |

**Testing Gate**: Full checkout flow: add items → checkout → `BasketCheckoutInitiated` event → Order created → `OrderCreated` event → Basket cleared → Order appears in order history.

---

### Phase 5 — Decommission the Monolith Web Layer (Week 13–14)

**Goal**: The monolith is now just a thin shell. Replace it with a proper BFF (Backend for Frontend) or a fully client-side frontend.

| Task | Detail |
|---|---|
| Audit remaining monolith controllers | Each should now only contain `HttpClient` calls to the APIs |
| Extract all Razor Views to a new `Web.BFF` project | Or convert to a React/Angular SPA calling the APIs directly |
| Remove all direct `IRepository`, `IAsyncRepository` dependencies from Web layer | Web only speaks HTTP |
| Remove `ApplicationCore` and `Infrastructure` project references from Web | Web is now infrastructure-agnostic |
| Update API Gateway | Point all BFF routes to the new `Web.BFF` |
| Archive old monolith solution | Do not delete — keep for rollback reference |

---

## Part 5 — Cross-Cutting Concerns

### 5.1 Authentication & Authorization

| Concern | Approach |
|---|---|
| Token Format | JWT (Bearer) with short expiry (15 min) + refresh tokens |
| Token Validation | Each service validates JWT independently (no Identity API call per request) |
| User Identity in Services | Extract `sub` (userId) and `email` claims from JWT |
| Role-based Access | Admin roles encoded in JWT claims |
| Anonymous Basket | Cookie-based anonymous ID, transferred on `UserLoggedIn` event |

### 5.2 Resilience & Fault Tolerance

| Concern | Library | Pattern |
|---|---|---|
| HTTP call failures | `Microsoft.Extensions.Http.Resilience` (Polly v8) | Retry with exponential backoff |
| Service unavailability | Polly `CircuitBreaker` | Stop calling a failing service temporarily |
| Message broker down | Outbox Pattern | Persist events to DB before publishing |
| Basket read failure | Fallback | Return empty basket rather than error page |

### 5.3 Observability

| Concern | Tool |
|---|---|
| Distributed Tracing | OpenTelemetry → Jaeger or Zipkin |
| Centralized Logging | Serilog → Seq or Elastic Stack (ELK) |
| Metrics | OpenTelemetry Metrics → Prometheus + Grafana |
| Health Checks | `Microsoft.Extensions.Diagnostics.HealthChecks` on each service |
| Correlation IDs | Propagate `X-Correlation-Id` header across all HTTP and event calls |

### 5.4 Service Discovery & Configuration

| Concern | Approach |
|---|---|
| Service URLs | Environment variables / `appsettings.json` per environment |
| Secrets | Azure Key Vault or HashiCorp Vault (never in source code) |
| Service Discovery | Docker Compose DNS (dev), Kubernetes Service DNS (prod) |

### 5.5 Data Consistency

| Scenario | Strategy |
|---|---|
| Basket checkout → Order creation | Eventual consistency via events (saga pattern) |
| Price change → Basket update | Eventual consistency; stale price shown until event processed |
| Order cancellation → Basket restore | Compensating transaction (future scope) |

---

## Part 6 — Infrastructure & DevOps Plan

### 6.1 Containerization

Each microservice gets its own `Dockerfile`. A root `docker-compose.yml` orchestrates:
- `catalog.api` container
- `basket.api` container
- `order.api` container
- `identity.api` container
- `api-gateway` container (YARP)
- `web-bff` container
- `rabbitmq` container (with management plugin)
- `redis` container
- `sqlserver-catalog` container
- `sqlserver-orders` container
- `sqlserver-identity` container

### 6.2 Kubernetes (Production Target)

| Resource | Purpose |
|---|---|
| `Deployment` per service | Scalable pod replicas |
| `Service` per service | Internal DNS-based discovery |
| `Ingress` | Expose API Gateway externally with TLS |
| `ConfigMap` | Non-secret configuration per service |
| `Secret` | Database passwords, JWT signing keys |
| `HorizontalPodAutoscaler` | Scale Basket and Catalog APIs under load |

### 6.3 CI/CD Pipeline (GitHub Actions)

```
On Pull Request:
  → Build all services
  → Run unit + integration tests
  → Docker build (no push)

On Merge to main:
  → Build all services
  → Run all tests
  → Docker build + push to registry
  → Deploy to staging environment
  → Run smoke tests
  → Manual approval gate
  → Deploy to production (rolling update)
```

---

## Part 7 — Risk Register

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| `TransferBasketAsync` is `NotImplementedException` | High | Medium | Must implement before extracting Basket service (Phase 3) |
| Anonymous basket loss on login | High | High | Implement `UserLoggedIn` event handler first |
| Data inconsistency during dual-write period | Medium | High | Use the Strangler Fig approach — never write to both old and new simultaneously |
| Message broker outage | Low | High | Implement Outbox pattern for critical events |
| Increased network latency | High | Medium | Cache catalog data in Basket service; use Redis for basket reads |
| Team unfamiliar with event-driven patterns | Medium | Medium | Start with sync HTTP calls, introduce events in Phase 3+ |
| InMemory DB in current code hides SQL issues | Medium | Medium | Migrate to real SQL Server early in each phase |

---

## Part 8 — Effort Estimate Summary

| Phase | Duration | Complexity |
|---|---|---|
| Phase 0: Foundation | 2 weeks | 🟡 Medium |
| Phase 1: Identity Service | 2 weeks | 🔴 High |
| Phase 2: Catalog Service | 2 weeks | 🟢 Low |
| Phase 3: Basket Service | 3 weeks | 🔴 High |
| Phase 4: Order Service | 3 weeks | 🔴 High |
| Phase 5: Decommission Monolith | 2 weeks | 🟡 Medium |
| **Total** | **~14 weeks** | — |

> These estimates assume a 1–2 developer team. Parallel work across phases (where dependencies allow) can compress the timeline to ~10 weeks.

---

## Part 9 — Recommended Technology Stack

| Concern | Technology | Rationale |
|---|---|---|
| Service Framework | ASP.NET Core 9 Minimal APIs | Lightweight, fast, built-in DI |
| API Gateway | YARP (Microsoft) | Native .NET, easy JWT validation, active support |
| Message Broker | RabbitMQ (dev) / Azure Service Bus (prod) | Battle-tested, great .NET client |
| Basket Storage | Redis via `StackExchange.Redis` | Purpose-built for volatile session data |
| Catalog/Order DB | SQL Server or PostgreSQL | Relational, matches current EF Core model |
| Identity | ASP.NET Core Identity + custom JWT | Avoids Duende licensing cost for now |
| Container Orchestration | Docker Compose (dev) → Kubernetes (prod) | Standard industry path |
| Resilience | `Microsoft.Extensions.Http.Resilience` (Polly v8) | First-class .NET 9 support |
| Observability | OpenTelemetry + Serilog + Seq | All open-source, unified approach |
| Testing | xUnit + `Microsoft.AspNetCore.Mvc.Testing` | Consistent with current test framework |

---

## Quick Reference — Service Dependency Map

```
Identity API  ──publishes──▶  UserLoggedIn  ──▶  Basket API
Catalog API   ──publishes──▶  CatalogItemPriceChanged  ──▶  Basket API
Basket API    ──calls sync──▶  Catalog API  (add-to-cart enrichment)
Basket API    ──publishes──▶  BasketCheckoutInitiated  ──▶  Order API
Order API     ──publishes──▶  OrderCreated  ──▶  Basket API (clear basket)
API Gateway   ──routes──▶  All services
Web BFF       ──calls──▶  All APIs via Gateway
```
