# Notification UX Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Platform REUSE:** `useToast` / `toastService` · email/SMS via Notifications module · push via mobile `core/notifications`

Mapping reference: [notification-mapping.md](../notification-mapping.md)

---

## 1. In-app toasts (web — REUSE)

Use existing toast system ([toasts/usage.md](../../../frontend/toasts/usage.md)):

| Variant | When | Duration | Example |
|---------|------|----------|---------|
| `success` | Mutation succeeded | 5s | "Lead converted successfully." |
| `error` | API/network failure | 8s + correlation | "Unable to save site. Reference: {id}" |
| `warning` | Reversible caution | 6s | "This will cancel the scheduled visit." |
| `info` | Neutral information | 5s | "Report submitted for admin review." |

**Queue:** Max 5 visible — platform default.

### CCTV success messages (standard copy)

| Action | Message |
|--------|---------|
| Create entity | "{Entity} created successfully." |
| Update entity | "Changes saved." |
| Lead convert | "Lead converted. Customer {number} created." |
| Assign engineer | "Engineer assigned. Notifications sent." |
| Visit submit | "Visit report submitted for approval." |
| Visit approve | "Visit report approved. Customer can now view." |
| Visit return | "Report returned to engineer for rework." |
| Ticket reopen | "Ticket reopened successfully." |
| Invoice generate | "Invoice generated. PDF ready." |
| Renewal request | "Renewal request submitted." |
| Offline sync | "All pending items synced." (mobile) |

---

## 2. In-app error messages

| Scenario | UX |
|----------|-----|
| Validation (400) | Field errors + toast "Please fix the highlighted fields." |
| Business rule (422) | Toast with `detail` from ProblemDetails |
| Forbidden (403) | Toast "You don't have permission for this action." |
| Not found (404) | Toast "Item not found or no longer available." |
| Conflict (409) | Toast "Record was updated by someone else. Refreshing…" + auto refetch |
| Network | Toast "Connection problem" + correlation if available |
| Offline sync reject | Per-item error in Sync Status list (mobile) |

---

## 3. Warning / confirm dialogs

Use `PlatformConfirmDialog` (REUSE) before:

| Action | Warning text |
|--------|--------------|
| Cancel schedule | "Cancel this visit? The customer will be notified." |
| Cancel invoice | "Cancel invoice {number}? This cannot be undone." |
| Deactivate customer | "Deactivate customer? Active contracts may block this." |
| Delete attachment | "Remove this attachment?" |
| Discard offline queue item | "Discard unsynced visit data?" (mobile) |

---

## 4. Email templates (EXTEND — content in `Templates/cctv/`)

| Template key | Subject line | Body highlights |
|--------------|--------------|-----------------|
| `cctv-lead-created` | New lead: {{LeadNumber}} | Source, contact, requirement summary, admin portal link |
| `cctv-lead-converted` | Lead converted: {{LeadNumber}} | Customer/site/contract refs |
| `cctv-customer-welcome` | Welcome to Aarvii AMC | Portal login, support contact |
| `cctv-ticket-created` | Ticket {{TicketNumber}} received | Subject, priority, portal link |
| `cctv-ticket-assigned` | Ticket {{TicketNumber}} assigned | Engineer name, expected response |
| `cctv-ticket-closed` | Ticket {{TicketNumber}} closed | Resolution summary |
| `cctv-visit-scheduled` | Visit scheduled on {{VisitDate}} | Site, engineer, time window |
| `cctv-visit-completed` | Service visit completed | Site, date, view report link (if approved) |
| `cctv-amc-expiry-reminder` | AMC expiring in {{Days}} days | Contract ref, renewal link |
| `cctv-invoice-generated` | Invoice {{InvoiceNumber}} | Amount, due date, download link |

Format: platform template file (Subject first line, body follows) — REUSE engine.

**User opt-out:** Email respects `emailNotificationsEnabled` — REUSE preferences page.

---

## 5. SMS templates (EXTEND — short text)

| Event | SMS text (≤160 chars) |
|-------|----------------------|
| Login OTP | "Your Aarvii login code is {{OtpCode}}. Valid 10 min." (platform) |
| Password reset OTP | "Aarvii password reset code: {{OtpCode}}." (platform) |
| Ticket assigned (High/Critical) | "Aarvii: Ticket {{TicketNumber}} assigned. Check app for details." |
| Visit scheduled | "Aarvii: Visit on {{VisitDate}} at {{SiteName}}. Engineer: {{EngineerName}}." |
| AMC expiry 30-day | "Aarvii: AMC for {{SiteName}} expires {{ExpiryDate}}. Request renewal in portal." |

SMS for OTP/security is **not** opt-out in V1.

---

## 6. Push notifications (mobile — EXTEND)

| Event | Title | Body | Deep link |
|-------|-------|------|-----------|
| Ticket assigned | New ticket | {{TicketNumber}}: {{Subject}} | `/tickets/{id}` |
| Visit scheduled | Visit scheduled | {{SiteName}} on {{Date}} | `/visits/{id}` |
| Visit completed | Visit completed | View service report | `/visits/history/{id}` |
| Invoice generated | New invoice | {{InvoiceNumber}} — ₹{{Amount}} | `/invoices/{id}` |
| AMC expiry | AMC reminder | Expires in {{Days}} days | `/amc` |

Tap → REUSE deep link handler → CCTV feature route.

---

## 7. Customer in-app notification feed (#58 — EXTEND)

| Aspect | Design |
|--------|--------|
| Layout | `PlatformCard` list or mobile list tile |
| Item | Icon by type · title · relative time · read/unread dot |
| Empty | "No notifications yet." |
| Source | Polling or push-triggered refresh V1 |

---

## 8. Classification

| UX element | Class |
|------------|:-----:|
| Toast system | REUSE |
| Confirm dialogs | REUSE |
| Email delivery | REUSE |
| CCTV email templates | EXTEND |
| SMS provider + templates | EXTEND |
| Push infrastructure | REUSE |
| Push payload wiring | EXTEND |
| In-app feed UI | EXTEND |

---

Related: [workflow-screen-design.md](./workflow-screen-design.md) · [validation-rules.md](./validation-rules.md)
