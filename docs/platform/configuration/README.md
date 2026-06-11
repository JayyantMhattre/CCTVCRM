# Platform configuration validation

Fail-fast startup validation prevents silent misconfiguration in production.

**Implementation:** `EnvironmentValidationExtensions.ValidateAshraakEnvironment()`

Called from `Program.cs` after service registration, before `Build()`.

## Related docs

- [required-env.md](./required-env.md)
- [validation.md](./validation.md)
- [troubleshooting.md](./troubleshooting.md)
