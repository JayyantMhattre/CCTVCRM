using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.CreatePlan;

public sealed record CreatePlanCommand(
    Guid TenantId,
    Guid UserId,
    string Code,
    string Name,
    string? Description) : IRequest<Result<AmcPlanDetailDto>>;
