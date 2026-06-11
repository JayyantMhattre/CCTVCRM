-- ─────────────────────────────────────────────────────────────────────────────
-- Ashraak — PostgreSQL Schema Initialisation
--
-- This script is executed by the PostgreSQL container on FIRST startup only
-- (when the data volume is empty). Subsequent container restarts skip it.
--
-- Purpose:
--   Create the per-module database schemas so EF Core migrations have a clean
--   target to apply into. Each module owns its own schema for isolation.
--
-- Schema → Module mapping:
--   auth   → Ashraak.Auth.Infrastructure   (Identity, OpenIddict tables)
--   tenant → Ashraak.Tenant.Infrastructure (Tenant, TenantSettings tables)
--   users  → Ashraak.Users.Infrastructure  (UserProfile, UserPreferences tables)
--
-- Audit data is stored in MongoDB (not PostgreSQL) — no schema needed here.
--
-- After this script runs, EF Core `dotnet ef database update` (or the auto-
-- migrate option configured in Program.cs) applies the actual table DDL.
-- ─────────────────────────────────────────────────────────────────────────────

-- Auth module schema
CREATE SCHEMA IF NOT EXISTS auth;

-- Tenant module schema
CREATE SCHEMA IF NOT EXISTS tenant;

-- Users module schema
CREATE SCHEMA IF NOT EXISTS users;

-- Files module schema
CREATE SCHEMA IF NOT EXISTS files;

-- Webhooks module schema
CREATE SCHEMA IF NOT EXISTS webhooks;

-- API Keys module schema
CREATE SCHEMA IF NOT EXISTS apikeys;

-- Grant all privileges to the application user on each schema.
-- The POSTGRES_USER env var becomes the owner automatically, but explicit
-- grants are good practice for least-privilege setups where a read-only
-- user is later introduced.
GRANT ALL PRIVILEGES ON SCHEMA auth   TO current_user;
GRANT ALL PRIVILEGES ON SCHEMA tenant TO current_user;
GRANT ALL PRIVILEGES ON SCHEMA users  TO current_user;
GRANT ALL PRIVILEGES ON SCHEMA files  TO current_user;

-- Set a descriptive comment for each schema (visible in pg_catalog).
COMMENT ON SCHEMA auth   IS 'ASP.NET Core Identity + OpenIddict tables for the Auth module.';
COMMENT ON SCHEMA tenant IS 'Tenant entity and settings tables for the Tenant module.';
COMMENT ON SCHEMA users  IS 'User profile and preferences tables for the Users module.';

-- Outbox tables (also created by EF migrations when applied)
CREATE TABLE IF NOT EXISTS auth.outbox_messages (
    "Id" uuid PRIMARY KEY,
    "Type" character varying(500) NOT NULL,
    "Content" text NOT NULL,
    "CreatedOnUtc" timestamp with time zone NOT NULL,
    "ProcessedOnUtc" timestamp with time zone NULL,
    "Error" character varying(2000) NULL
);

CREATE TABLE IF NOT EXISTS tenant.outbox_messages (
    "Id" uuid PRIMARY KEY,
    "Type" character varying(500) NOT NULL,
    "Content" text NOT NULL,
    "CreatedOnUtc" timestamp with time zone NOT NULL,
    "ProcessedOnUtc" timestamp with time zone NULL,
    "Error" character varying(2000) NULL
);

CREATE TABLE IF NOT EXISTS auth.invitations (
    id uuid PRIMARY KEY,
    tenant_id uuid NOT NULL,
    email varchar(320) NOT NULL,
    role varchar(100) NOT NULL,
    token_hash varchar(128) NOT NULL,
    status varchar(32) NOT NULL,
    expires_on_utc timestamptz NOT NULL,
    invited_by_user_id uuid NOT NULL,
    accepted_by_user_id uuid NULL,
    created_on_utc timestamptz NOT NULL,
    updated_on_utc timestamptz NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_auth_invitations_token_hash ON auth.invitations (token_hash);

CREATE TABLE IF NOT EXISTS auth.user_sessions (
    id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    created_on_utc timestamptz NOT NULL,
    last_used_on_utc timestamptz NOT NULL,
    ip_address varchar(64) NOT NULL,
    user_agent varchar(512) NOT NULL,
    is_revoked boolean NOT NULL DEFAULT false,
    revoked_on_utc timestamptz NULL
);

CREATE INDEX IF NOT EXISTS ix_auth_user_sessions_user_tenant ON auth.user_sessions (user_id, tenant_id, is_revoked);

CREATE TABLE IF NOT EXISTS files.file_records (
    id uuid PRIMARY KEY,
    tenant_id uuid NOT NULL,
    file_name varchar(500) NOT NULL,
    content_type varchar(200) NOT NULL,
    size bigint NOT NULL,
    storage_path varchar(1000) NOT NULL,
    uploaded_by uuid NOT NULL,
    uploaded_on_utc timestamptz NOT NULL,
    deleted_on_utc timestamptz NULL
);

CREATE INDEX IF NOT EXISTS ix_files_file_records_tenant ON files.file_records (tenant_id);

CREATE TABLE IF NOT EXISTS files.outbox_messages (
    "Id" uuid PRIMARY KEY,
    "Type" character varying(500) NOT NULL,
    "Content" text NOT NULL,
    "CreatedOnUtc" timestamp with time zone NOT NULL,
    "ProcessedOnUtc" timestamp with time zone NULL,
    "Error" character varying(2000) NULL
);

CREATE TABLE IF NOT EXISTS users.outbox_messages (
    "Id" uuid PRIMARY KEY,
    "Type" character varying(500) NOT NULL,
    "Content" text NOT NULL,
    "CreatedOnUtc" timestamp with time zone NOT NULL,
    "ProcessedOnUtc" timestamp with time zone NULL,
    "Error" character varying(2000) NULL
);

CREATE TABLE IF NOT EXISTS webhooks.webhook_subscriptions (
    id uuid PRIMARY KEY,
    tenant_id uuid NOT NULL,
    name varchar(200) NOT NULL,
    endpoint_url varchar(2000) NOT NULL,
    secret_protected varchar(2000) NOT NULL,
    enabled boolean NOT NULL DEFAULT true,
    subscribed_event_names jsonb NOT NULL DEFAULT '[]',
    created_by uuid NOT NULL,
    created_on_utc timestamptz NOT NULL,
    updated_on_utc timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS webhooks.webhook_deliveries (
    id uuid PRIMARY KEY,
    subscription_id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    event_name varchar(200) NOT NULL,
    event_version varchar(20) NOT NULL,
    correlation_id varchar(100) NULL,
    payload text NOT NULL,
    attempt_number int NOT NULL DEFAULT 1,
    retry_count int NOT NULL DEFAULT 0,
    status varchar(32) NOT NULL,
    response_code int NULL,
    response_body varchar(4000) NULL,
    last_failure_reason varchar(500) NULL,
    last_failure_code int NULL,
    next_retry_on_utc timestamptz NULL,
    started_on_utc timestamptz NOT NULL,
    completed_on_utc timestamptz NULL
);

CREATE INDEX IF NOT EXISTS ix_webhooks_deliveries_tenant_started
    ON webhooks.webhook_deliveries (tenant_id, started_on_utc DESC);
CREATE INDEX IF NOT EXISTS ix_webhooks_deliveries_subscription_started
    ON webhooks.webhook_deliveries (subscription_id, started_on_utc DESC);
CREATE INDEX IF NOT EXISTS ix_webhooks_deliveries_status_next_retry
    ON webhooks.webhook_deliveries (status, next_retry_on_utc);

CREATE TABLE IF NOT EXISTS webhooks.webhook_dead_letters (
    id uuid PRIMARY KEY,
    delivery_id uuid NOT NULL,
    subscription_id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    event_name varchar(200) NOT NULL,
    payload text NOT NULL,
    failure_reason varchar(500) NULL,
    failure_code int NULL,
    retry_count int NOT NULL DEFAULT 0,
    correlation_id varchar(100) NULL,
    created_on_utc timestamptz NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_webhooks_dead_letters_tenant_created
    ON webhooks.webhook_dead_letters (tenant_id, created_on_utc DESC);
CREATE INDEX IF NOT EXISTS ix_webhooks_dead_letters_delivery
    ON webhooks.webhook_dead_letters (delivery_id);

CREATE UNIQUE INDEX IF NOT EXISTS ix_webhooks_subscriptions_tenant_name
    ON webhooks.webhook_subscriptions (tenant_id, name);

CREATE TABLE IF NOT EXISTS webhooks.webhook_event_definitions (
    id uuid PRIMARY KEY,
    event_name varchar(200) NOT NULL,
    version varchar(20) NOT NULL DEFAULT 'v1',
    description varchar(1000) NOT NULL,
    enabled boolean NOT NULL DEFAULT true
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_webhooks_event_definitions_name
    ON webhooks.webhook_event_definitions (event_name);

CREATE TABLE IF NOT EXISTS webhooks.outbox_messages (
    "Id" uuid PRIMARY KEY,
    "Type" character varying(500) NOT NULL,
    "Content" text NOT NULL,
    "CreatedOnUtc" timestamp with time zone NOT NULL,
    "ProcessedOnUtc" timestamp with time zone NULL,
    "Error" character varying(2000) NULL
);

-- API Keys tables (safety net before EF migrations)
CREATE TABLE IF NOT EXISTS apikeys.api_keys (
    id uuid PRIMARY KEY,
    tenant_id uuid NOT NULL,
    name varchar(200) NOT NULL,
    description varchar(1000) NOT NULL DEFAULT '',
    key_prefix varchar(64) NOT NULL,
    hashed_secret varchar(512) NOT NULL,
    environment varchar(16) NOT NULL DEFAULT 'prod',
    scopes jsonb NOT NULL DEFAULT '[]',
    created_by uuid NOT NULL,
    created_on_utc timestamptz NOT NULL,
    expires_on_utc timestamptz NULL,
    last_used_on_utc timestamptz NULL,
    revoked_on_utc timestamptz NULL,
    enabled boolean NOT NULL DEFAULT true,
    request_count bigint NOT NULL DEFAULT 0,
    success_count bigint NOT NULL DEFAULT 0,
    failure_count bigint NOT NULL DEFAULT 0,
    last_correlation_id varchar(128) NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_apikeys_tenant_name
    ON apikeys.api_keys (tenant_id, name);
CREATE UNIQUE INDEX IF NOT EXISTS ix_apikeys_key_prefix
    ON apikeys.api_keys (key_prefix);

CREATE TABLE IF NOT EXISTS apikeys.outbox_messages (
    "Id" uuid PRIMARY KEY,
    "Type" character varying(500) NOT NULL,
    "Content" text NOT NULL,
    "CreatedOnUtc" timestamp with time zone NOT NULL,
    "ProcessedOnUtc" timestamp with time zone NULL,
    "Error" character varying(2000) NULL
);
