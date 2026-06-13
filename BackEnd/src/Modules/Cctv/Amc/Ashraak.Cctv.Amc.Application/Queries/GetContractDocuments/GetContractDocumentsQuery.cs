using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetContractDocuments;

public sealed record GetContractDocumentsQuery(
    Guid TenantId,
    Guid UserId,
    Guid ContractId) : IRequest<Result<IReadOnlyList<AmcContractDocumentDto>>>;
