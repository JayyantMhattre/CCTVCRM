# Configuration validation

## When it runs

During host construction:

```csharp
builder.ValidateAshraakEnvironment();
```

Throws `InvalidOperationException` with a bullet list of missing/invalid settings — process exits before listening on ports.

## Validated rules

1. All required connection strings non-empty
2. `Outbox:PollInterval` parseable and positive
3. `Outbox:BatchSize` positive integer
4. `Notifications:Provider` and `TemplatesPath` set
5. Production: `Seq:Url` and signing key present

## Extending validation

Add checks to `EnvironmentValidationExtensions.cs` and document new keys in [required-env.md](./required-env.md).

Do not add runtime-only checks here — use health checks for dependency reachability.
