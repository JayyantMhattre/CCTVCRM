namespace Ashraak.Cctv.Integration.Application.Rbac;

/// <summary>
/// CCTV role names seeded into the platform Auth RBAC store (Sprint 0).
/// </summary>
public static class CctvRoles
{
    public const string Engineer = "Engineer";
    public const string Customer = "Customer";
}

/// <summary>
/// All 30 NEW CCTV business permissions from the approved permission catalog (D0-5).
/// </summary>
public static class CctvPermissions
{
    public static readonly IReadOnlyList<string> All =
    [
        "leads:read", "leads:manage", "leads:convert",
        "customers:read", "customers:manage",
        "sites:read", "sites:manage",
        "amcplans:read", "amcplans:manage",
        "amc:read", "amc:manage", "amc:request-renewal",
        "schedules:read", "schedules:manage",
        "visits:assign", "visits:read", "visits:execute", "visits:approve",
        "tickets:read", "tickets:create", "tickets:assign", "tickets:update", "tickets:close", "tickets:reopen",
        "engineers:read", "engineers:manage",
        "invoices:read", "invoices:manage", "invoices:download",
        "reports:read"
    ];

    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> RolePermissionMap =
        new Dictionary<string, IReadOnlyList<string>>
        {
            ["Admin"] = All,
            [CctvRoles.Engineer] =
            [
                "files:read", "files:write",
                "schedules:read", "visits:read", "visits:execute",
                "tickets:read", "tickets:create", "tickets:update"
            ],
            [CctvRoles.Customer] =
            [
                "files:read", "files:write",
                "sites:read", "amc:read", "amc:request-renewal",
                "schedules:read", "visits:read",
                "tickets:read", "tickets:create", "tickets:update", "tickets:reopen",
                "invoices:read", "invoices:download"
            ]
        };
}
