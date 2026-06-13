namespace Ashraak.Cctv.Service.Application;

internal static class SchedulePermissions
{
    public const string Read = "schedules:read";
    public const string Manage = "schedules:manage";
}

internal static class VisitPermissions
{
    public const string Assign = "visits:assign";
    public const string Read = "visits:read";
    public const string Execute = "visits:execute";
    public const string Approve = "visits:approve";
}
