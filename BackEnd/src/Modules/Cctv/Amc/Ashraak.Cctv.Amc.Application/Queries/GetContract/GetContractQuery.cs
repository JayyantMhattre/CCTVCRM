using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetContract;

public sealed record GetContractQuery(
    Guid TenantId,
    Guid UserId,
    Guid ContractId) : IRequest<Result<AmcContractDetailDto>>;
