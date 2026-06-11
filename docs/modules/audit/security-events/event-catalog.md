# Security Event Catalog

Defined in `Auth.Domain/Aggregates/AuthUser/Events/SecurityAuditDomainEvents.cs`:

| Domain event | Trigger |
|--------------|---------|
| `UserLoggedInDomainEvent` | Successful login |
| `FailedLoginDomainEvent` | Failed login attempt (user known) |
| `InvalidPasswordDomainEvent` | Wrong password |
| `AccountLockedDomainEvent` | 5 failed attempts → 15 min lock |
| `AccountUnlockedDomainEvent` | Successful login after lock |
| `UserPasswordChangedDomainEvent` | Password change |
| `PasswordResetRequestedDomainEvent` | `RequestPasswordReset()` |
| `MfaEnabledDomainEvent` | `EnableMfa()` |
| `MfaDisabledDomainEvent` | `DisableMfa()` |
| `TokenRevokedDomainEvent` | `RevokeToken()` |
| `RevokeAllSessionsDomainEvent` | `RevokeAllSessions()` |
| `UserRegisteredDomainEvent` | User registration |

Audit `Action` column uses event type name with `DomainEvent`/`Event` suffix stripped.
