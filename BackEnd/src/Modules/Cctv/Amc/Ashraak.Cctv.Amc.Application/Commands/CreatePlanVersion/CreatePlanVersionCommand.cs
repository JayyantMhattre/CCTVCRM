using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.CreatePlanVersion;

public sealed record CreatePlanVersionCommand(
    Guid TenantId,
    Guid UserId,
    Guid PlanId,
    decimal Price,
    int VisitFrequency,
    IReadOnlyList<string> IncludedServices,
    string SlaDescription,
    DateOnly EffectiveFrom) : IRequest<Result<AmcPlanVersionDetailDto>>;
