# Security Audit — Sample Queries

MongoDB (`ashraak_audit.audit_entries`):

```javascript
// Failed logins for tenant
db.audit_entries.find({
  TenantId: "<tenant-guid>",
  Action: "FailedLogin"
}).sort({ OccurredOnUtc: -1 }).limit(50)

// Account locks
db.audit_entries.find({ Action: "AccountLocked" })

// Logins with IP in NewValues JSON
db.audit_entries.find({ Action: "UserLoggedIn" })
```

HTTP query API (`GET /api/v1/audit-logs`) remains stub — use Mongo shell or Phase 2 read model.
