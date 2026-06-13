using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.ActivateContractTerm;

public sealed record ActivateContractTermCommand(
    Guid TenantId,
    Guid UserId,
    Guid ContractId,
    Guid TermId,
    uint RowVersion) : IRequest<Result<AmcContractDetailDto>>;
