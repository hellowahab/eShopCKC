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
- Test project exists: `Ckc.EShop.ApplicationCore.Tests`
- Run tests: `dotnet test Ckc.EShop.ApplicationCore.Tests`
- Test framework: xUnit
- Current test coverage: Basic property validation tests for all entities

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

### Test Coverage Status

#### Entity Test Status (ApplicationCore.Tests):
✓ BaseEntityTests.cs - Tests Id property
✓ BasketTests.cs - Tests Id, BuyerId properties
✓ BasketItemsTests.cs - Tests Id, UnitPrice, Quantity, CatalogItemId properties
✓ CatalogBrandTests.cs - Tests Id, Brand properties (null/empty handling)
✓ CatalogItemTests.cs - Tests Id, Name, Description, Price, PictureUri, CatalogBrandId, CatalogTypeId properties
✓ CatalogTypeTests.cs - Tests Id, Type properties (null/empty handling)

#### Test Project Details:
- Project: Ckc.EShop.ApplicationCore.Tests
- Framework: xUnit with .NET 10.0
- References: Ckc.EShop.ApplicationCore
- Test pattern: Property validation (default values and assignment)

#### Improvement Roadmap for Testing:

**Short-Term Improvements:**
1. **Enhanced Entity Testing**
   - Add boundary value tests (negative numbers, zero values, max/min values)
   - Test validation if data annotations are added to entities
   - Test equality/comparison operations if implemented
   - Test constructor overloading scenarios

2. **Relationship Testing**
   - Test entity relationships (Basket-BasketItems, CatalogItem-CatalogBrand/CatalogType)
   - Test navigation properties if implemented
   - Test aggregate root patterns

3. **Service Layer Tests**
   - Create tests for ApplicationCore services
   - Mock repository dependencies
   - Test business logic implementations

**Medium-Term Improvements:**
4. **Infrastructure Testing**
   - Test EF Core repositories with InMemory provider
   - Test database mapping and migrations
   - Test identity infrastructure

5. **Integration Testing**
   - Test service/repository interactions
   - Test API controller endpoints
   - Test end-to-end user flows

**Long-Term Improvements:**
6. **Advanced Testing Patterns**
   - Implement test fixtures for shared setup
   - Use theory data tests for multiple scenarios
   - Add mocking framework (Moq) for dependency isolation
   - Implement test categorization (unit, integration, functional)

7. **Test Automation**
   - Add test reporting and code coverage
   - Configure test execution in CI/CD pipelines
   - Add performance/load testing scenarios

### Current Testing Guidelines:
- All entity tests follow `{EntityName}Tests.cs` naming convention
- Test methods follow `{PropertyName}_{Scenario}_{ExpectedResult}` pattern
- Focus on property validation (default values and assignment)
- Use xUnit attributes: [Fact] for simple tests, [Theory] for parameterized tests
- Maintain AAA pattern (Arrange, Act, Assert) in all tests