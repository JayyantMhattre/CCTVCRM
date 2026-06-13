using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.CreateContract;

public sealed record CreateContractCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    Guid CustomerId,
    Guid PlanVersionId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price) : IRequest<Result<AmcContractDetailDto>>;
