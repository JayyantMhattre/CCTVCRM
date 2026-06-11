# Notification preferences — Flow

```mermaid
sequenceDiagram
    participant User
    participant Page as NotificationPreferencesPage
    participant API as Users API

    User->>Page: Open /users/me/notifications
    Page->>API: GET /users/{id}
    API-->>Page: UserDto.preferences
    User->>Page: Toggle email + Save
    Page->>API: PATCH /users/{id}/preferences
    API-->>Page: Updated preferences
    Page->>User: Success toast
```

## Navigation

- Sidebar: **Notifications** (all authenticated users)
- User profile: **Manage** link next to email notifications status

## Field

| UI | DTO field |
|----|-----------|
| Email notifications enabled | `preferences.emailNotificationsEnabled` |
