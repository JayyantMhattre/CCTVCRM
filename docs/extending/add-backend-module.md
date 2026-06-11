# Add a Backend Module

Follow the **Billing** walkthrough in [DEVELOPER_GUIDE.md](../../DEVELOPER_GUIDE.md) §5 — this page summarizes the contract.

---

## Checklist

1. Create four projects: `{Product}.{Name}.Domain|Application|Infrastructure|Api`
2. Add projects to `Ashraak.slnx`
3. Implement aggregate + repository + `*Module.cs` + `*Endpoints.cs`
4. Wire in `ModuleExtensions.cs`:
   - `services.Add{Name}Module(configuration);`
   - `routeBuilder.Map{Name}Endpoints();`
5. Add `<ProjectReference>` to `Ashraak.Api.csproj`
6. Add connection string + PostgreSQL schema in `init-db.sql`
7. Generate EF migration (when migrations are adopted)
8. Create **full** `docs/modules/{name}/` (7 files per governance)
9. Update `docs/architecture/module-map.md`

---

## Registration order

| Layer | When to register |
|-------|------------------|
| 0 | Shared infra (caching) — before Auth |
| 1 | Identity (Auth) |
| 2 | Business modules (Tenant, Users, your module) |
| 3 | Observers (Audit) — **last** |

---

## DbContext pattern

```csharp
services.AddDbContext<YourDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__ef_migrations_history", "your_schema");
    });
    options.AddInterceptors(sp.GetServices<IInterceptor>()); // Audit
});
services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<YourDbContext>());
```

---

## Cross-module access

Use `SharedKernel.Contracts` only — never reference another module's Infrastructure.

---

## Example modules in template

| Module | Docs |
|--------|------|
| Audit (observer) | [modules/audit](../modules/audit/README.md) |
| Auth (identity) | [modules/auth](../modules/auth/README.md) |

---

## Related

- [add-contracts-and-handlers.md](./add-contracts-and-handlers.md)
- [Documentation governance](../documentation-governance.md)
