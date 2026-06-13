using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.LinkContractDocument;

public sealed record LinkContractDocumentCommand(
    Guid TenantId,
    Guid UserId,
    Guid ContractId,
    Guid FileId,
    string DocumentType,
    string Title,
    Guid? TermId) : IRequest<Result<AmcContractDocumentDto>>;
