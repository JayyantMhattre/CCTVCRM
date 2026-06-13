using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPlan;

public sealed record GetPlanQuery(
    Guid TenantId,
    Guid UserId,
    Guid PlanId) : IRequest<Result<AmcPlanDetailDto>>;
