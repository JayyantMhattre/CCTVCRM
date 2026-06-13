using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPlanVersion;

public sealed record GetPlanVersionQuery(
    Guid TenantId,
    Guid UserId,
    Guid PlanId,
    Guid VersionId) : IRequest<Result<AmcPlanVersionDetailDto>>;
