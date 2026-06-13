using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.CreateContractTerm;

public sealed record CreateContractTermCommand(
    Guid TenantId,
    Guid UserId,
    Guid ContractId,
    Guid PlanVersionId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price,
    string TermType) : IRequest<Result<AmcContractTermDetailDto>>;
