# Notifications — Sample Templates

Shipped under `BackEnd/src/Host/Ashraak.Api/Templates/`:

| File | Template key | Placeholders |
|------|--------------|--------------|
| welcome.txt | `welcome` | DisplayName, TenantId, UserId, TenantName, Slug, Plan |
| invitation.txt | `invitation` | InvitationToken, TenantId, ExpiresOnUtc |
| verification.txt | `verification` | VerificationCode |
| password-reset.txt | `password-reset` | ResetToken |
| tenant-suspended.txt | `tenant-suspended` | TenantId, Reason |

Add new templates by creating `{key}.txt` and a constant in `EmailTemplates.cs`.
