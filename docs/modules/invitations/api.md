# Invitations API

## Create

```http
POST /api/v1/auth/invitations
Authorization: Bearer {token}
Content-Type: application/json

{ "email": "user@example.com", "role": "Member", "expiryDays": 7 }
```

Response includes `invitationId` and one-time `token` (dev only — production should email link only).

## Accept

```http
POST /api/v1/auth/invitations/accept
{ "token": "...", "password": "...", "displayName": "Jane" }
```
