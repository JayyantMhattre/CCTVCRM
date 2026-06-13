using Ashraak.SharedKernel.Interfaces;

namespace Ashraak.Cctv.Integration.Application;

public static class CctvTenantHelper
{
    public static Guid ResolveTenantId(ITenantContext tenantContext, CctvNotificationOptions options) =>
        tenantContext.TenantId != Guid.Empty ? tenantContext.TenantId : options.DefaultTenantId;
}
