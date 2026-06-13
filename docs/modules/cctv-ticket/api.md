# API — Ticket Management

Base path: `/api/v1/cctv` · Feature flag: `cctv.tickets.enabled`

## Admin / shared

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/tickets` | `tickets:read` | Paginated list (admin filters; customer/engineer scoped) |
| GET | `/tickets/{ticketId}` | `tickets:read` | Ticket detail with comments, attachments, history |
| POST | `/tickets` | `tickets:create` | Create ticket (V-TKT-03 site ownership for customers) |
| POST | `/tickets/{ticketId}/assign` | `tickets:assign` | Assign engineer (Open/Reopened → Assigned) |
| PATCH | `/tickets/{ticketId}/status` | `tickets:update` | InProgress / Resolved transitions |
| POST | `/tickets/{ticketId}/comments` | `tickets:update` | Add comment |
| POST | `/tickets/{ticketId}/attachments` | `tickets:create` | Link file attachment (max 5) |
| POST | `/tickets/{ticketId}/close` | `tickets:close` | Close from Resolved (admin) |
| POST | `/tickets/{ticketId}/reopen` | `tickets:reopen` | Reopen from Closed (customer, reason ≥ 10 chars) |

## Portal

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/portal/tickets` | Customer's tickets for linked customer profile |

## Engineer

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/engineer/tickets` | Tickets with active assignment to logged-in engineer |

DTOs: `docs/project/design/dto-catalog.md` § Tickets.
