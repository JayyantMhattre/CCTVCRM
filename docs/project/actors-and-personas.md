# Actors and Personas

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md §3](./requirements-freeze-v1.md) (Actors), §2 (Applications), §15 (Engineer Rules)

Four actors are approved for V1. Persona narratives below add context for design work; **permitted actions are exactly those in the freeze document** — personas grant no additional capabilities.

---

## 1. Public Visitor

> *"I need a reliable CCTV AMC provider — show me what you offer and let me ask for a quote quickly."*

| Attribute | Description |
|-----------|-------------|
| Who | Anonymous user of [www.aarvii.in](http://www.aarvii.in) — typically a business owner, facility/office manager, or homeowner evaluating CCTV maintenance |
| Application | Public Website only (no login) |

### Goals
- Understand Aarvii's services and AMC plans.
- See proof of credibility (gallery, testimonials).
- Get a quotation or raise an AMC inquiry with minimal friction.

### Responsibilities
- Provide accurate contact details when submitting inquiries or quote requests.

### Pain points
- Doesn't know what an AMC includes or which plan fits.
- Slow callbacks after submitting interest elsewhere.
- No visibility of whether an inquiry was received.

### System interactions (freeze §3)
| Interaction | Module |
|-------------|--------|
| Browse Home, About Us, Services, AMC Services, Contact Us, Gallery, Testimonials | Public Website |
| Submit contact inquiry / AMC Inquiry | Public Website → Lead Management (auto-creates lead, §10) |
| Request quotation (Get Quote) | Public Website → Lead Management |
| Navigate to Login (for existing users) | Public Website → Auth |

---

## 2. Customer

> *"I've paid for an AMC — I want to see what I'm getting: visits done, what's due, my invoices, and a quick way to complain when something breaks."*

| Attribute | Description |
|-----------|-------------|
| Who | AMC contract holder (often via a converted lead); may operate multiple sites |
| Applications | Customer Portal (web) + Customer App (Flutter) |

### Goals
- Know the current AMC term status and when it expires.
- See upcoming visits and the history of completed (approved) service.
- Raise complaint tickets and track them to closure; reopen if unresolved.
- Download invoices; request AMC renewal on time.

### Responsibilities
- Keep profile and site contact persons (max 3 per site, §6) current.
- Confirm completed work by signing on the engineer's device (§12).
- Report faults via tickets rather than ad-hoc channels.

### Pain points
- No visibility of contract coverage or renewal dates → unexpected lapses.
- Can't prove or verify whether maintenance visits happened.
- Complaints disappear into phone calls; no status tracking.
- Chasing the vendor for invoice copies.

### System interactions (freeze §3)
| Interaction | Module |
|-------------|--------|
| View own AMC contracts (current active term, §8) | AMC Contracts / Customer Portal |
| View service history & upcoming visits (approved reports only, §13) | Visit Management / Customer Portal |
| Raise tickets; reopen closed tickets (§14) | Ticket Management |
| View & download invoices (PDF, §16) | Invoice Management |
| Request AMC renewal | AMC Contracts |
| Update profile; password reset (OTP, §17) | Customer Portal / Auth |
| Receive Email/SMS notifications (§17) | Notifications |

---

## 3. Engineer

> *"Tell me where to go and what to do today — and let me file my report from the site, even with bad network."*

| Attribute | Description |
|-----------|-------------|
| Who | Aarvii field service engineer executing preventive maintenance visits and ticket work |
| Applications | Engineer Portal (web) + Engineer App (Flutter, with offline support §18) |

### Goals
- See assigned visits and tickets clearly, day by day.
- Complete visit reporting quickly on-site: photos, selfie, GPS, customer signature, remarks.
- Log new faults found during a visit as tickets immediately.

### Responsibilities
- Attend assigned visits as scheduled (§11).
- Capture all mandatory completion evidence: selfie, GPS coordinates, ≥1 photo, customer signature, remarks (§12).
- Upload before/during/after photos and reports honestly (§12, §15).
- Create tickets for issues observed during visits (§3, §14).

### Pain points
- Paper job sheets get lost; rework when office can't read reports.
- Disputes about whether a visit happened or what was done.
- Poor connectivity at customer sites blocks report submission.

### System interactions (freeze §3, §15)
| Interaction | Module |
|-------------|--------|
| View assigned visits & tickets | Service Scheduling / Ticket Management / Engineer Portal |
| Submit visit report (→ admin review, §13) | Visit Management |
| Upload photos (before/during/after) & videos (§12, §15) | Visit Management / Files |
| Upload selfie; capture GPS (lat/long/timestamp, §12) | Visit Management |
| Capture customer signature (§12) | Visit Management |
| Create tickets during visits (§14) | Ticket Management |

### Explicit restrictions (freeze §15)
Engineers **cannot**: manage customers, manage AMC plans, manage contracts.

---

## 4. Admin

> *"I run the operation — leads in, contracts signed, visits done and verified, tickets closed, invoices paid. I need one console for all of it."*

| Attribute | Description |
|-----------|-------------|
| Who | Aarvii Technologies back-office / operations staff |
| Application | Admin Portal |

### Goals
- Convert leads into customers with sites and initial AMC contracts (§10).
- Keep AMC plans, contracts, and renewal terms accurate (§8, §9).
- Ensure every AMC's owed visits are scheduled, assigned, and completed (§11).
- Verify field evidence and approve reports before customers see them (§13).
- Keep tickets moving to resolution; issue invoices per contract term (§14, §16).

### Responsibilities
- Manage all modules (§3): leads, customers, sites, assets, AMC plans, contracts, scheduling, tickets, engineers, invoices, reporting.
- Review and approve engineer visit reports (§13).
- Reschedule visits and assign engineers (assignment is mandatory, §11).
- Generate and send invoices linked to AMC contract terms (§16).
- Monitor operations via Reporting.

### Pain points
- Leads from the website leak without a tracked pipeline.
- Renewals missed because contract history isn't centralized.
- No proof of field work → customer disputes.
- Manual invoice tracking against contract years.

### System interactions (freeze §3)
| Interaction | Module |
|-------------|--------|
| Manage lead pipeline; convert Won leads | Lead Management |
| Manage customers, sites (≤3 contacts), asset summaries | Customer / Site / Asset Management |
| Manage AMC plans (versioned, §9) and contracts/terms (§8) | AMC Plans / AMC Contracts |
| Generate/reschedule visits; assign engineers (§11) | Service Scheduling |
| Review & approve visit reports (§13) | Visit Management |
| Manage tickets (create, assign, progress, close, §14) | Ticket Management |
| Manage engineers | Engineer Management |
| Manage invoices (Draft→Paid/Cancelled, PDF, §16) | Invoice Management |
| View reports | Reporting |

---

## Actor × application matrix

| Actor | Public Website | Customer Portal (web) | Customer App | Engineer Portal (web) | Engineer App | Admin Portal |
|-------|:---:|:---:|:---:|:---:|:---:|:---:|
| Public Visitor | ✅ | — | — | — | — | — |
| Customer | ✅ (pre-login) | ✅ | ✅ | — | — | — |
| Engineer | — | — | — | ✅ | ✅ | — |
| Admin | — | — | — | — | — | ✅ |

---

## Related documents

- [requirements-freeze-v1.md](./requirements-freeze-v1.md)
- [business-requirements-document.md](./business-requirements-document.md)
- [navigation-map.md](./navigation-map.md)
- [screen-inventory.md](./screen-inventory.md)
