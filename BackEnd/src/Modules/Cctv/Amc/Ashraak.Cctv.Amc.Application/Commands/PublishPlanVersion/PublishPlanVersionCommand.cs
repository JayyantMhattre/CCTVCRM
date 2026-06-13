using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.PublishPlanVersion;

public sealed record PublishPlanVersionCommand(
    Guid TenantId,
    Guid UserId,
    Guid PlanId,
    Guid VersionId,
    uint RowVersion) : IRequest<Result<AmcPlanVersionDetailDto>>;
