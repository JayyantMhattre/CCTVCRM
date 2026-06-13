using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.RetirePlan;

public sealed record RetirePlanCommand(
    Guid TenantId,
    Guid UserId,
    Guid PlanId,
    uint RowVersion) : IRequest<Result<AmcPlanDetailDto>>;
