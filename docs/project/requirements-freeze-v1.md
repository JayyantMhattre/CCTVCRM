# Aarvii CCTV AMC Management System

# Requirements Freeze V1

# Authoritative Source of Truth

Version: 1.0
Status: Approved & Frozen
Date: Initial Baseline

---

# 1. Project Vision

Build a centralized CCTV AMC Management System for Aarvii Technologies that manages:

* Public website
* Lead generation
* Customers
* Sites
* AMC contracts
* Preventive maintenance schedules
* Service visits
* Complaint tickets
* Engineers
* Invoices
* Customer self-service portal
* Engineer portal
* Mobile applications

The platform will provide complete visibility of customer AMC lifecycle, engineer activities, service history, contract renewals, invoices, and complaint resolution.

---

# 2. Applications

The solution consists of four business applications sharing a common backend.

## Public Website

Domain:

[www.aarvii.in](http://www.aarvii.in)

Purpose:

* Business showcase
* Lead generation
* AMC plan information
* Contact inquiries
* Quote requests

Existing public website content should be reused wherever possible.

Pages:

* Home
* About Us
* Services
* AMC Services
* Contact Us
* Gallery
* Testimonials
* Login

Enhancements:

* Get Quote
* AMC Inquiry

---

## Customer Portal

Web + Mobile

Features:

* Dashboard
* AMC Details
* Service History
* Upcoming Visits
* Tickets
* Invoices
* Profile Management
* Password Reset

---

## Engineer Portal

Web + Mobile

Features:

* Assigned Visits
* Assigned Tickets
* Visit Reporting
* Photo Upload
* GPS Capture
* Selfie Capture
* Customer Signature
* Ticket Creation

---

## Admin Portal

Features:

* Lead Management
* Customer Management
* Site Management
* Asset Management
* AMC Plans
* AMC Contracts
* Scheduling
* Ticket Management
* Engineer Management
* Invoice Management
* Reporting

---

# 3. Actors

## Public Visitor

Can:

* Browse website
* Submit inquiries
* Request quotations

---

## Customer

Can:

* View own AMC contracts
* View invoices
* View service history
* Raise tickets
* Reopen closed tickets
* Request AMC renewal
* Update profile

---

## Engineer

Can:

* View assigned work
* Create tickets during visits
* Upload visit reports
* Upload visit photos
* Upload selfie
* Capture GPS location
* Capture customer signature

---

## Admin

Can:

* Manage all modules
* Approve visit reports
* Manage contracts
* Manage invoices
* Manage engineers
* Manage customers

---

# 4. Approved Modules

1. Public Website
2. Lead Management
3. Customer Management
4. Site Management
5. Asset Management
6. AMC Plans
7. AMC Contracts
8. Service Scheduling
9. Visit Management
10. Ticket Management
11. Engineer Management
12. Invoice Management
13. Reporting
14. Customer Portal
15. Engineer Portal

---

# 5. Customer Structure

Customer
└── Site
├── Contact Persons (Maximum 3)
├── Asset Summary
├── AMC Contract
├── Scheduled Visits
├── Tickets
└── Invoices

---

# 6. Site Rules

* One site belongs to one customer.
* A customer can have multiple sites.
* A site can have a maximum of three contact persons.
* One site can have only one active AMC contract at a time.

---

# 7. Asset Strategy

Assets are tracked as summary counts per site.

The system will not track individual cameras.

Supported asset counts:

* Camera Count
* DVR Count
* NVR Count
* Hard Disk Count
* Switch Count
* Router Count
* Monitor Count

Optional:

* Brand
* Model
* Remarks

---

# 8. AMC Contract Strategy

AMC Contract uses a Master + Terms model.

## AMC Contract

Permanent contract record.

## AMC Contract Term

Renewal history records.

Example:

Contract
├── Term 2026
├── Term 2027
└── Term 2028

Customer sees:

* Current active term

Admin sees:

* Complete renewal history

---

# 9. AMC Plan Rules

Examples:

* Silver
* Gold
* Platinum

Plan stores:

* Price
* Visit Frequency
* Included Services
* SLA

Plan versioning is required.

Historical contracts must not change when plans are modified.

---

# 10. Lead Lifecycle

Statuses:

* New
* Contacted
* Qualified
* Quotation Sent
* Negotiation
* Won
* Lost
* Converted

Website inquiries automatically create leads.

Converted leads create:

* Customer
* Site
* Initial AMC Contract

---

# 11. Service Scheduling Lifecycle

Statuses:

* Planned
* Assigned
* In Progress
* Completed
* Missed
* Cancelled

Visits are automatically generated from AMC frequency.

Admin may reschedule visits.

Engineer assignment is mandatory.

---

# 12. Service Visit Rules

Mandatory before completion:

* Engineer Selfie
* GPS Coordinates
* Minimum One Visit Photo
* Customer Signature
* Visit Remarks

Stored:

* Latitude
* Longitude
* Timestamp

Supported photos:

* Before Photos
* During Photos
* After Photos

---

# 13. Visit Approval Workflow

Engineer
↓
Submit Visit Report
↓
Admin Review
↓
Approve
↓
Customer Can View

Customer cannot view reports before approval.

---

# 14. Ticket Lifecycle

Statuses:

* Open
* Assigned
* In Progress
* Resolved
* Closed
* Reopened

Priorities:

* Low
* Medium
* High
* Critical

Rules:

* Customer may create ticket.
* Admin may create ticket.
* Engineer may create ticket.
* Customer may reopen ticket.

---

# 15. Engineer Rules

Engineers can:

* View assigned work
* Upload photos
* Upload videos
* Upload reports
* Create tickets

Engineers cannot:

* Manage customers
* Manage AMC plans
* Manage contracts

---

# 16. Invoice Lifecycle

Statuses:

* Draft
* Generated
* Sent
* Paid
* Cancelled

Rules:

* Invoice linked to AMC Contract Term.
* Customer can download invoice.
* PDF generation required.

No accounting features in V1.

---

# 17. Notifications

Channels:

* Email
* SMS

Events:

* Lead Created
* Lead Converted
* Ticket Created
* Ticket Assigned
* Ticket Closed
* Visit Scheduled
* Visit Completed
* AMC Expiry Reminder
* Invoice Generated
* Password Reset OTP
* Login OTP

---

# 18. Mobile Applications

Flutter

## Customer App

* Dashboard
* AMC
* Tickets
* Invoices
* Notifications
* Profile

## Engineer App

* Visits
* Tickets
* Photo Upload
* GPS Capture
* Signature Capture
* Offline Support

---

# 19. PDF Documents

Generate:

* AMC Contract PDF
* Visit Report PDF
* Invoice PDF

---

# 20. Reuse Existing Platform Capabilities

Reuse existing base platform:

* Authentication
* Roles
* Permissions
* Files
* Notifications
* Audit
* Theme Engine
* Mobile Foundation
* API Keys
* Webhooks

No duplicate implementation.

---

# 21. Out of Scope (V1)

Not included:

* Inventory Stock Management
* Purchase Orders
* Vendor Management
* Accounting
* GST Filing
* ERP
* CRM
* Payroll
* Attendance
* Geo Tracking
* WhatsApp Integration
* Payment Gateway
* AI Features

---

# 22. Scope Freeze Declaration

This document is the approved and frozen baseline for V1.

All future BRD, HLD, ERD, LLD, APIs, UI Designs, Mobile Designs, and Development Phases must follow this document.

Requirements may only change through an approved change request process.
