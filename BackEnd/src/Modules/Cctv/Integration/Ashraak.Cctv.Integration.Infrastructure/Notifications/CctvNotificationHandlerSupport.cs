using Ashraak.Cctv.Integration.Application;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Notifications;

internal static class CctvNotificationHandlerSupport
{
    public const string DeepLinkKey = "DeepLink";

    public static Guid ResolveTenant(ITenantContext tenantContext, IOptions<CctvNotificationOptions> options) =>
        CctvTenantHelper.ResolveTenantId(tenantContext, options.Value);

    public static Dictionary<string, string> Data(params (string Key, string? Value)[] pairs)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in pairs)
            map[key] = value ?? string.Empty;
        return map;
    }

    public static string DeepLink(string relativePath) =>
        $"ashraak://{relativePath.TrimStart('/')}";

    public static string CustomerTicketDeepLink(Guid ticketId) =>
        DeepLink($"cctv/customer/tickets/{ticketId:D}");

    public static string CustomerInvoiceDeepLink(Guid invoiceId) =>
        DeepLink($"cctv/customer/invoices/{invoiceId:D}");

    public static string CustomerVisitsDeepLink() => DeepLink("cctv/customer/visits");

    public static string CustomerServiceHistoryDeepLink() => DeepLink("cctv/customer/service-history");

    public static string CustomerAmcDeepLink() => DeepLink("cctv/customer/amc");

    public static string EngineerVisitReportDeepLink(Guid visitId) =>
        DeepLink($"cctv/engineer/visits/{visitId:D}/report");

    public static string EngineerVisitsDeepLink() => DeepLink("cctv/engineer/visits");

    public static string EngineerTicketsDeepLink() => DeepLink("cctv/engineer/tickets");
}
