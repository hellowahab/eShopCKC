# SECURITY.md

## Overview

This repository follows a **defense‑in‑depth** security model that covers the full application stack—from the ASP.NET Core web layer down to the data access layer. The goal is to protect against the OWASP Top 10 risks, satisfy compliance requirements (GDPR, CCPA), and ensure that any new code adheres to the security standards defined here.

---

## 1. Threat Modeling

| Threat | Mitigation |
|--------|------------|
| **Injection (SQL/NoSQL)** | Use Entity Framework Core with parameterised queries only. No raw SQL strings. Enable EF Core's `UseInMemoryDatabase` for tests and configure `DbContext` with `EnableSensitiveDataLogging(false)` in production. |
| **Broken Authentication** | ASP.NET Core Identity is the default authentication provider. Enforce strong password policies, lock‑out after failed attempts, and enable multi‑factor authentication (MFA) where possible. |
| **Sensitive Data Exposure** | Store secrets (connection strings, API keys) in a secure vault (Azure Key Vault, HashiCorp Vault) and reference them via `IConfiguration`. Do not hard‑code secrets. |
| **Security Misconfiguration** | Adopt the **Microsoft Security Code Analysis** GitHub Action, enforce HTTPS, enable HSTS, and remove default developer error pages in production (`UseExceptionHandler`). |
| **Cross‑Site Scripting (XSS)** | Razor views automatically HTML‑encode output. For any raw HTML strings, use `HtmlEncoder.Default.Encode`. |
| **Insecure Deserialization** | Avoid binary serialization. Prefer JSON with `System.Text.Json` using safe options (`PropertyNameCaseInsensitive = false`). |
| **Insufficient Logging & Monitoring** | Integrate Serilog with structured logging and ship logs to a centralized system (Seq, ELK). Log security‑relevant events (login failures, privilege changes). |
| **Broken Access Control** | Enforce role‑based access controls (RBAC) using ASP.NET Core policies. Validate user claims on every controller/action. |
| **Using Components with Known Vulnerabilities** | Run `dotnet list package --outdated` regularly and apply updates via a Dependabot workflow. |
| **Insufficient Security Testing** | Add static analysis (SonarCloud, Brakeman), dynamic scanning (OWASP ZAP), and regular penetration tests. |

---

## 2. Secure Development Guidelines

1. **Code Reviews**
   - All PRs must pass the **Security Review Checklist** (see `SECURITY_CHECKLIST.md`).
   - Spot‑check for:
     - Direct string concatenation in queries.
     - Hard‑coded secrets.
     - Missing `[ValidateAntiForgeryToken]` on POST actions.
2. **Static Analysis**
   - Enable the `Microsoft.CodeAnalysis.FxCopAnalyzers` NuGet package.
   - Treat warnings as errors in CI (`TreatWarningsAsErrors=true`).
3. **Dependency Management**
   - Use the **Dependabot** GitHub bot for automatic security updates.
   - Periodically run `dotnet audit` and resolve identified CVEs.
4. **Input Validation**
   - Validate all external input using Data Annotations and custom validators.
   - Prefer *whitelisting* over blacklisting.
5. **Error Handling**
   - Do **not** expose stack traces or internal details to the client.
   - Use `app.UseExceptionHandler("/Error")` for production.
6. **Secure Configuration**
   - Keep `appsettings.Development.json` out of source control (`*.json` in `.gitignore`).
   - Use `User Secrets` for local dev and environment variables for production.

---

## 3. Authentication & Authorization

- **Identity**: ASP.NET Core Identity with email confirmation and lock‑out.
- **Password Policy**:
  ```csharp
  options.Password.RequireDigit = true;
  options.Password.RequiredLength = 12;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequireUppercase = true;
  options.Password.RequireLowercase = true;
  ```
- **MFA**: Enable `IdentityOptions.SignIn.RequireTwoFactor = true` for sensitive accounts.
- **Authorization Policies**:
  ```csharp
  services.AddAuthorization(options =>
  {
      options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
      options.AddPolicy("RequireCustomer", policy => policy.RequireClaim("CustomerId"));
  });
  ```

---

## 4. Logging & Monitoring

| Component | Tool | Purpose |
|-----------|------|---------|
| Application logs | **Serilog** → Seq/ELK | Structured logs, correlation IDs |
| Security events | **Audit.NET** | Track authentication/authorization events |
| Performance metrics | **Prometheus** + **Grafana** | Alert on anomalies (e.g., rapid request spikes) |
| Exception tracking | **Sentry** or **AppInsights** | Real‑time error visibility |

---

## 5. Security Testing

1. **Static Application Security Testing (SAST)**
   - Run `dotnet format` with security analyzers in CI.
   - Use `dotnet build -warnaserror` to treat analyzer warnings as failures.
2. **Dynamic Application Security Testing (DAST)**
   - Execute an OWASP ZAP baseline scan against the local dev URL (`https://localhost:5001`).
   - Fail the CI pipeline if any *high* severity alerts are found.
3. **Dependency Scanning**
   - `dotnet list package --vulnerable` via a scheduled GitHub Action.
4. **Penetration Testing**
   - Schedule quarterly manual pentests. Record findings in the `SECURITY_REPORTS/` folder.

---

## 6. Incident Response

| Step | Description |
|------|-------------|
| **Detect** | Monitor alerts from Sentry/Grafana. |
| **Contain** | Revoke compromised tokens, isolate affected services. |
| **Eradicate** | Apply security patches, rotate secrets. |
| **Recover** | Deploy clean build from a trusted branch, verify integrity. |
| **Post‑mortem** | Document root cause, update `SECURITY.md` and any related policies. |

---

## 7. References

- OWASP Top 10 – https://owasp.org/www-project-top-ten/
- Microsoft Secure Development Lifecycle – https://learn.microsoft.com/en-us/security/sdl/
- ASP.NET Core Security Documentation – https://learn.microsoft.com/aspnet/core/security/

*This file should be reviewed and approved by the security lead before merging to `master`.*