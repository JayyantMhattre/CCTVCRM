using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.CancelContract;

public sealed record CancelContractCommand(
    Guid TenantId,
    Guid UserId,
    Guid ContractId,
    uint RowVersion) : IRequest<Result<AmcContractDetailDto>>;
