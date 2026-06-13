using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Reporting.Application.Queries.GetAdminDashboard;

public sealed record GetAdminDashboardQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<AdminDashboardDto>>;
