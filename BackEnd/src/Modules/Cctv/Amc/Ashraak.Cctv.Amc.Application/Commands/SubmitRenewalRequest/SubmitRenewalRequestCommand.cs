using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.SubmitRenewalRequest;

public sealed record SubmitRenewalRequestCommand(
    Guid TenantId,
    Guid UserId,
    Guid ContractId,
    string? Message) : IRequest<Result<AmcContractDetailDto>>;
