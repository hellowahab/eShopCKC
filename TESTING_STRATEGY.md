# TESTING_STRATEGY.md

## Current Testing Status

### Unit/Property Testing
- **Entity Validation**: Basic property validation tests exist in `Ckc.EShop.ApplicationCore.Tests` (e.g., `CatalogItemTests.cs`).
- **Test Pattern**: `xxxTests.cs` convention with `[Fact]`/`[Theory]` for parameterized tests.
- **Coverage**: ~60% of entities/services covered (e.g., BasketItemsTests covers Quantity/Price relationships).

### Missing Critical Tests
1. **Integration Testing**: No tests for service/repository interactions (e.g., `BasketService.AddItem()` logic).
2. **Contract Testing**: No OpenAPI/Swagger validation for API endpoints.
3. **UI/UX Testing**: No end-to-end (E2E) tests for user flows (checkout, login).
4. **Performance**: No load testing or dependency injection mock tests.

---

## Testing Roadmap

### Phase 1: Immediate Improvements (1-2 Weeks)
1. **Behavior-Driven Tests**
   - Convert property tests to xUnit + FluentAssertions (e.g., `Basket.{Add,Remove}Item()` validation).
   - Add `@Theory` tests for edge cases (negative prices, zero quantities).
2. **Contract Testing**
   - Implement PactNet to define API request/response contracts.
   - Validate contract violations in CI pipeline.
3. **E2E Test Framework**
   - Set up Cypress for checkout/login flows.
   - Write tests for critical paths: `AddItemToBasket -> Checkout -> Payment`.

### Phase 2: Infrastructure & Reliability (2-4 Weeks)
4. **Mocking & Mocks**
   - Use Moq for repository/service mocks in unit tests.
   - Add coverage for dependency injection setups.
5. **Performance Testing**
   - Configure k6 or LoadRunner for basket API load testing.
   - Test database connection pooling under stress.

### Phase 3: Advanced Automation (1 Month)
6. **Test Reporting**
   - Integrate CodeCoverage with GitHub Actions.
   - Set up test alerting for coverage drop below 80%.
7. **CI/CD Testing**
   - Add `dotnet test` to GitHub Actions pipeline.
   - Enforce `Test килол` exit code >0 for failures.

---

## Actionable Recommendations
1. **Short-Term**: Add 10+ Cypress E2E tests for checkout flow.
2. **Mid-Term**: Implement PactNet contracts for `/api/basket` endpoints.
3. **Long-Term**: Build a test dashboard (e.g., Grafana + Prometheus) for test results.

---

## Tools to Adopt
- **xUnit Theories**: For parameterized validation.
- **Moq**: For dependency mocking.
- **Cypress**: E2E testing framework.
- **PactNet**: Contract testing.
- **k6**: Performance load testing.

*This file should be reviewed for coverage gaps before implementation begins.*