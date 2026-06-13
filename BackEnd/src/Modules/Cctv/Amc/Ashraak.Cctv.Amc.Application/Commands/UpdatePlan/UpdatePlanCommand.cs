using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.UpdatePlan;

public sealed record UpdatePlanCommand(
    Guid TenantId,
    Guid UserId,
    Guid PlanId,
    string Name,
    string? Description,
    uint RowVersion) : IRequest<Result<AmcPlanDetailDto>>;
