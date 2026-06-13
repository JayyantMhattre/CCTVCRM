using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetEngineerDashboard;

public sealed record GetEngineerDashboardQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<EngineerDashboardDto>>;
