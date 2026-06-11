# Invitations module (Auth)

Tenant user invitations are implemented in the **Auth** module (`auth.invitations` table).

## API (v1)

| Method | Path | Auth |
|--------|------|------|
| POST | `/api/v1/auth/invitations` | `user:invite` or Admin |
| POST | `/api/v1/auth/invitations/{id}/resend` | same |
| POST | `/api/v1/auth/invitations/{id}/revoke` | same |
| POST | `/api/v1/auth/invitations/accept` | Anonymous |

## Events

`UserInvitedEvent` is published after create/resend → **Notifications** sends invitation email (outbox/async).

Domain events (`InvitationCreated`, `Resent`, `Revoked`, `Accepted`) are captured by **Audit**.

See [flows.md](./flows.md), [api.md](./api.md).
