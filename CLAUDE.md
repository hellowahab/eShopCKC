# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Setup

### Building the Project
- Build all projects: `dotnet build eShopCKC.sln`
- Clean and rebuild: `dotnet clean eShopCKC.sln && dotnet build eShopCKC.sln`

### Running the Application
- Run the web application: `dotnet run --project ckc.eShop.Web\Ckc.EShop.Web.csproj`
- The application will be accessible at https://localhost:5001 (or http://localhost:5000)

### Managing Dependencies
- Restore NuGet packages: `dotnet restore eShopCKC.sln`
- Add a package: `dotnet add <project> package <package-name>`
- Update packages: `dotnet list <project> package --outdated`

## Code Architecture

### Solution Structure
The solution follows Clean Architecture principles with three main projects:

1. **Ckc.EShop.ApplicationCore** - Contains business logic, entities, interfaces, and specifications
   - Entities: Domain models (CatalogItem, Order, Basket, etc.)
   - Interfaces: Repository contracts and service abstractions
   - Services: Business logic implementations
   - Specifications: Query filtering patterns

2. **Ckc.EShop.Infrastructure** - Data access and external concerns
   - Data: Entity Framework Core contexts and repositories
   - Identity: ASP.NET Core Identity implementation
   - Migrations: Database schema evolution

3. **ckc.eShop.Web** - Presentation layer and application entry point
   - Controllers: MVC controllers handling HTTP requests
   - Services: Application services (wrapping core services)
   - ViewModels: Data transfer objects for views
   - Program.cs: Application configuration and middleware setup

### Key Architectural Patterns
- **Repository Pattern**: Abstracts data access via `IRepository<T>` and `IAsyncRepository<T>`
- **Dependency Injection**: Services registered in `Program.cs` using built-in DI container
- **Specification Pattern**: Encapsulates business rules in reusable specifications
- **Unit of Work**: Implemented via EF Core DbContext instances
- **Clean Architecture Layers**: Separation of concerns between Core, Infrastructure, and Web

### Database Contexts
- `CatalogDbContext`: Manages catalog data (products, brands, types)
- `AppIdentityDbContext`: Handles ASP.NET Core Identity tables
- Both configured to use InMemoryDatabase for development (see Program.cs lines 46-78)

### Common Development Tasks

#### Running Tests
- No test project currently exists in the solution
- To add testing: Create xUnit test projects referencing ApplicationCore and Infrastructure

#### Database Operations
- Migrations are managed via EF Core
- To create migration: `dotnet ef migrations add <name> --project <project> --startup-project ckc.eShop.Web`
- To apply migration: `dotnet ef database update --project <project> --startup-project ckc.eShop.Web`

#### Working with Services
- Core services accessed via interfaces in ApplicationCore
- Web layer services wrap core services for presentation concerns
- Services registered with appropriate lifetimes (Scoped, Transient, Singleton)

### Important Files
- `Program.cs`: Application entry point and service configuration
- `Ckc.EShop.ApplicationCore\Entities\*`: Domain entities
- `Ckc.EShop.ApplicationCore\Interface\*`: Repository and service contracts
- `Ckc.EShop.Infrastructure\Data\*`: EF Core contexts and repositories
- `ckc.eShop.Web\Controllers\*`: MVC controllers
- `ckc.eShop.Web\ViewModels\*`: View-specific models

### Technology Stack
- **Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server (configured for InMemory in development)
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views with Bootstrap (implied from controllers returning views)

### Code Conventions
- C# 12 with .NET 9 features
- PascalCase for public members, camelCase for parameters/local variables
- Interfaces prefixed with 'I'
- Repository methods follow CRUD patterns
- Configuration via `IOptions<T>` pattern

### Improvement Roadmap
The following areas represent opportunities for enhancing this e-commerce application:

#### Short-Term Improvements
1. **Add Testing Infrastructure**
   - Create xUnit test projects for ApplicationCore, Infrastructure, and Web layers
   - Implement unit tests for business logic, specifications, and services
   - Add integration tests for key user flows (checkout, authentication)

2. **Enhance Error Handling & Validation**
   - Implement FluentValidation for model validation
   - Add global exception handling middleware
   - Improve API error responses with proper HTTP status codes

3. **Improve Security**
   - Add rate limiting to API endpoints
   - Implement security headers (CSP, HSTS, etc.)
   - Add input sanitization to prevent XSS/CSRF attacks

#### Medium-Term Improvements
4. **Production-Ready Database Configuration**
   - Replace InMemoryDatabase with proper SQL Server configuration
   - Add connection strings to appsettings.json with environment-based switching
   - Implement database migration strategies for production deployments

5. **API Documentation & Developer Experience**
   - Add Swagger/OpenAPI documentation for all endpoints
   - Include XML comments in controllers and models
   - Configure Swagger UI for development/testing environments

6. **Logging & Observability**
   - Implement structured logging with Serilog or built-in logging
   - Add correlation IDs for request tracing
   - Configure different log levels per environment

#### Long-Term Improvements
7. **Performance & Scalability**
   - Add response caching for appropriate endpoints
   - Implement Redis for distributed caching
   - Add database indexing strategies and query optimization

8. **Advanced Features**
   - Implement email confirmation and password reset flows
   - Add role-based authorization (admin/customer roles)
   - Implement product reviews and ratings system
   - Add wishlist and coupon/discount functionality

9. **DevOps & CI/CD**
   - Create GitHub Actions workflow for automated builds/tests
   - Add Docker support for containerization
   - Implement deployment scripts for various environments

10. **Monitoring & Analytics**
    - Add health check endpoints
    - Implement metrics collection (Prometheus/Grafana)
    - Add distributed tracing (OpenTelemetry)