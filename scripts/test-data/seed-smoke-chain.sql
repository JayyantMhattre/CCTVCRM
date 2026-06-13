-- ─────────────────────────────────────────────────────────────────────────────
-- Ashraak CCTV — Smoke Test Seed Data (TP-LOCAL-1)
--
-- Purpose:
--   Insert a minimal, repeatable entity graph for manual smoke testing.
--   Synthetic data only — no production PII.
--
-- Prerequisites:
--   1. PostgreSQL running with schemas from BackEnd/scripts/init-db.sql
--   2. EF Core migrations applied (see scripts/database/apply-migrations.ps1)
--   3. API started at least once (RBAC seed runs on startup)
--
-- Usage:
--   docker compose exec -T postgres psql -U ashraak -d ashraak \
--     < scripts/test-data/seed-smoke-chain.sql
--
-- Idempotent: uses ON CONFLICT DO NOTHING on primary keys.
-- ─────────────────────────────────────────────────────────────────────────────

BEGIN;

-- Fixed identifiers for repeatable smoke tests (see smoke-test-data-guide.md)
-- System actor (created_by): 22222222-2222-2222-2222-222222222222
-- Engineer platform user:     33333333-3333-3333-3333-333333333333
-- Customer portal user:       44444444-4444-4444-4444-444444444444

-- ── Lead (LEAD-01 New, LEAD-02 Qualified) ────────────────────────────────────

INSERT INTO cctv_lead.leads (
    id, lead_number, source, status, contact_name, organization_name,
    email, phone, city, address, requirement_summary,
    owner_user_id, created_at, created_by, is_deleted, row_version
) VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1',
        'LEAD-00001', 'Website', 'New',
        'Rajesh Kumar', 'Acme Retail Pvt Ltd',
        'acme.lead@test.local', '+919876543210',
        'Bengaluru', '100 MG Road, Bengaluru',
        'CCTV AMC for retail store',
        '22222222-2222-2222-2222-222222222222',
        NOW() - INTERVAL '5 days', '22222222-2222-2222-2222-222222222222',
        false, 1
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',
        'LEAD-00002', 'Referral', 'Qualified',
        'Priya Sharma', 'Beta Corp Industries',
        'beta.lead@test.local', '+919876543211',
        'Bengaluru', '200 Outer Ring Road, Bengaluru',
        'Premium AMC with 12 visits/year',
        '22222222-2222-2222-2222-222222222222',
        NOW() - INTERVAL '3 days', '22222222-2222-2222-2222-222222222222',
        false, 1
    )
ON CONFLICT (id) DO NOTHING;

-- ── Customer (CUST-01) ────────────────────────────────────────────────────────

INSERT INTO cctv_customer.customers (
    id, customer_number, name, email, phone,
    billing_address, city, status, portal_user_id, source_lead_id,
    created_at, created_by, is_deleted, row_version
) VALUES (
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
    'CUST-00001', 'Acme Retail Pvt Ltd',
    'customer@test.local', '+919876543210',
    '100 MG Road, Bengaluru', 'Bengaluru', 'Active',
    '44444444-4444-4444-4444-444444444444',
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',
    NOW() - INTERVAL '2 days', '22222222-2222-2222-2222-222222222222',
    false, 1
)
ON CONFLICT (id) DO NOTHING;

-- ── Site (SITE-01) ──────────────────────────────────────────────────────────

INSERT INTO cctv_customer.sites (
    id, customer_id, site_number, name, address, city, status,
    created_at, created_by, is_deleted, row_version
) VALUES (
    'cccccccc-cccc-cccc-cccc-ccccccccccc1',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
    'SITE-00001', 'Acme HQ Bangalore',
    '100 MG Road, Bengaluru', 'Bengaluru', 'Active',
    NOW() - INTERVAL '2 days', '22222222-2222-2222-2222-222222222222',
    false, 1
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO cctv_customer.site_contacts (
    id, site_id, contact_slot, name, designation, phone, email, is_primary,
    created_at, created_by
) VALUES (
    'cccccccc-cccc-cccc-cccc-ccccccccccc2',
    'cccccccc-cccc-cccc-cccc-ccccccccccc1',
    1, 'Priya Sharma', 'Site Manager',
    '+919876543211', 'priya.site@test.local', true,
    NOW() - INTERVAL '2 days', '22222222-2222-2222-2222-222222222222'
)
ON CONFLICT (id) DO NOTHING;

-- ── AMC Plan + Version (PLAN-PREM) ──────────────────────────────────────────

INSERT INTO cctv_amc.amc_plans (
    id, plan_code, name, description, status,
    created_at, created_by, is_deleted, row_version
) VALUES (
    'dddddddd-dddd-dddd-dddd-dddddddddd01',
    'PLAN-PREM', 'Premium AMC',
    '12 visits per year with priority SLA', 'Active',
    NOW() - INTERVAL '30 days', '22222222-2222-2222-2222-222222222222',
    false, 1
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO cctv_amc.amc_plan_versions (
    id, amc_plan_id, version_no, price, visit_frequency_per_year,
    included_services, sla_terms, effective_from, status, is_referenced,
    created_at, created_by
) VALUES (
    'dddddddd-dddd-dddd-dddd-dddddddddd02',
    'dddddddd-dddd-dddd-dddd-dddddddddd01',
    1, 25000.00, 12,
    'Preventive maintenance, remote support',
    'Priority response within 4 business hours',
    (CURRENT_DATE - INTERVAL '30 days')::date, 'Published', true,
    NOW() - INTERVAL '30 days', '22222222-2222-2222-2222-222222222222'
)
ON CONFLICT (id) DO NOTHING;

-- ── AMC Contract + Term (AMC-01) ────────────────────────────────────────────

INSERT INTO cctv_amc.amc_contracts (
    id, contract_number, site_id, customer_id, source_lead_id, status,
    created_at, created_by, row_version
) VALUES (
    'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1',
    'AMC-00001',
    'cccccccc-cccc-cccc-cccc-ccccccccccc1',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',
    'Active',
    NOW() - INTERVAL '30 days', '22222222-2222-2222-2222-222222222222', 1
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO cctv_amc.amc_contract_terms (
    id, amc_contract_id, term_no, amc_plan_version_id,
    start_date, end_date, agreed_price, status, origin,
    renewal_requested_by_customer, created_at, created_by, row_version
) VALUES (
    'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2',
    'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1',
    1, 'dddddddd-dddd-dddd-dddd-dddddddddd02',
    (CURRENT_DATE - INTERVAL '30 days')::date,
    (CURRENT_DATE + INTERVAL '335 days')::date,
    25000.00, 'Active', 'New',
    false,
    NOW() - INTERVAL '30 days', '22222222-2222-2222-2222-222222222222', 1
)
ON CONFLICT (id) DO NOTHING;

-- ── Engineer (ENG-01) ───────────────────────────────────────────────────────

INSERT INTO cctv_engineer.engineers (
    id, engineer_number, name, phone, status, platform_user_id,
    created_at, created_by, row_version
) VALUES (
    '20202020-2020-2020-2020-202020202001',
    'ENG-00001', 'Test Engineer',
    '+919876543299', 'Active',
    '33333333-3333-3333-3333-333333333333',
    NOW() - INTERVAL '10 days', '22222222-2222-2222-2222-222222222222', 1
)
ON CONFLICT (id) DO NOTHING;

-- ── Service Schedule + Visit (SCHED-01, VISIT-01) ───────────────────────────

INSERT INTO cctv_service.service_schedules (
    id, schedule_number, amc_contract_term_id, site_id,
    scheduled_date, sequence_in_term, status, is_auto_generated,
    created_at, created_by, row_version
) VALUES (
    'ffffffff-ffff-ffff-ffff-fffffffffff1',
    'SCHED-00001',
    'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2',
    'cccccccc-cccc-cccc-cccc-ccccccccccc1',
    (CURRENT_DATE + INTERVAL '3 days')::date,
    1, 'Assigned', true,
    NOW() - INTERVAL '7 days', '22222222-2222-2222-2222-222222222222', 1
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO cctv_service.engineer_assignments (
    id, service_schedule_id, engineer_id, assigned_by, assigned_at,
    is_active, created_at, created_by
) VALUES (
    'ffffffff-ffff-ffff-ffff-fffffffffff2',
    'ffffffff-ffff-ffff-ffff-fffffffffff1',
    '20202020-2020-2020-2020-202020202001',
    '22222222-2222-2222-2222-222222222222',
    NOW() - INTERVAL '2 days', true,
    NOW() - INTERVAL '2 days', '22222222-2222-2222-2222-222222222222'
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO cctv_service.service_visits (
    id, service_schedule_id, engineer_id,
    report_status, is_customer_visible,
    created_at, created_by, row_version
) VALUES (
    '10101010-1010-1010-1010-101010101001',
    'ffffffff-ffff-ffff-ffff-fffffffffff1',
    '20202020-2020-2020-2020-202020202001',
    'Draft', false,
    NOW() - INTERVAL '1 day', '22222222-2222-2222-2222-222222222222', 1
)
ON CONFLICT (id) DO NOTHING;

-- ── Ticket (TKT-01 Open) ────────────────────────────────────────────────────

INSERT INTO cctv_ticket.tickets (
    id, ticket_number, customer_id, site_id, amc_contract_id,
    source, subject, description, priority, status, reopen_count,
    created_at, created_by, is_deleted, row_version
) VALUES (
    '30303030-3030-3030-3030-303030303001',
    'TKT-00001',
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
    'cccccccc-cccc-cccc-cccc-ccccccccccc1',
    'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1',
    'Customer', 'Camera offline at main entrance',
    'Customer reports main entrance camera showing no signal since morning.',
    'Medium', 'Open', 0,
    NOW() - INTERVAL '6 hours', '44444444-4444-4444-4444-444444444444',
    false, 1
)
ON CONFLICT (id) DO NOTHING;

-- ── Invoices (INV-01 Draft, INV-03 Sent, INV-04 Paid) ───────────────────────

INSERT INTO cctv_invoice.invoices (
    id, invoice_number, customer_id, site_id, invoice_type,
    amc_contract_term_id, invoice_date, due_date,
    subtotal_amount, tax_amount, total_amount, status,
    created_at, created_by, row_version
) VALUES
    (
        '40404040-4040-4040-4040-404040404001',
        'INV-00001',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
        'cccccccc-cccc-cccc-cccc-ccccccccccc1',
        'NewAmc',
        'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2',
        CURRENT_DATE,
        (CURRENT_DATE + INTERVAL '15 days')::date,
        25000.00, 4500.00, 29500.00, 'Draft',
        NOW() - INTERVAL '1 day', '22222222-2222-2222-2222-222222222222', 1
    ),
    (
        '40404040-4040-4040-4040-404040404003',
        'INV-00003',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
        'cccccccc-cccc-cccc-cccc-ccccccccccc1',
        'NewAmc',
        'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2',
        (CURRENT_DATE - INTERVAL '10 days')::date,
        (CURRENT_DATE - INTERVAL '5 days')::date,
        25000.00, 4500.00, 29500.00, 'Sent',
        NOW() - INTERVAL '10 days', '22222222-2222-2222-2222-222222222222', 1
    ),
    (
        '40404040-4040-4040-4040-404040404004',
        'INV-00004',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
        'cccccccc-cccc-cccc-cccc-ccccccccccc1',
        'AmcRenewal',
        'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2',
        (CURRENT_DATE - INTERVAL '30 days')::date,
        (CURRENT_DATE - INTERVAL '15 days')::date,
        25000.00, 4500.00, 29500.00, 'Paid',
        NOW() - INTERVAL '30 days', '22222222-2222-2222-2222-222222222222', 1
    )
ON CONFLICT (id) DO NOTHING;

INSERT INTO cctv_invoice.invoice_lines (
    id, invoice_id, line_no, description, quantity, unit_price, line_total,
    created_at, created_by
) VALUES
    (
        '40404040-4040-4040-4040-404040404011',
        '40404040-4040-4040-4040-404040404001',
        1, 'Premium AMC — Annual', 1, 25000.00, 25000.00,
        NOW() - INTERVAL '1 day', '22222222-2222-2222-2222-222222222222'
    ),
    (
        '40404040-4040-4040-4040-404040404033',
        '40404040-4040-4040-4040-404040404003',
        1, 'Premium AMC — Annual', 1, 25000.00, 25000.00,
        NOW() - INTERVAL '10 days', '22222222-2222-2222-2222-222222222222'
    ),
    (
        '40404040-4040-4040-4040-404040404044',
        '40404040-4040-4040-4040-404040404004',
        1, 'Premium AMC — Renewal', 1, 25000.00, 25000.00,
        NOW() - INTERVAL '30 days', '22222222-2222-2222-2222-222222222222'
    )
ON CONFLICT (id) DO NOTHING;

COMMIT;
