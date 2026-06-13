using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Reporting.Application.Queries.GetAdminDashboard;

internal sealed class GetAdminDashboardQueryHandler(
    ICctvReportingDataProvider dataProvider,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
{
    public async Task<Result<AdminDashboardDto>> Handle(
        GetAdminDashboardQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ReportingEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Reports.Disabled", "Reporting is not enabled for this tenant.");

        var authError = await ReportingAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var dashboard = await dataProvider.GetAdminDashboardAsync(cancellationToken);
        return dashboard;
    }
}
