namespace Ashraak.Cctv.Lead.Application;

/// <summary>CCTV lead RBAC permission constants (mirrors <c>CctvPermissions</c>).</summary>
internal static class LeadPermissions
{
    public const string Read = "leads:read";
    public const string Manage = "leads:manage";
    public const string Convert = "leads:convert";
}
