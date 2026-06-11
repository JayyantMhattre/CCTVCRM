# Invitations architecture

- **Aggregate:** `Invitation` in `Auth.Domain`
- **Token:** SHA-256 hash stored; plain token only in `UserInvitedEvent` for email link
- **Role assignment:** `IRoleAssignmentService` on accept
- **No duplicate audit pipeline** — domain events only
