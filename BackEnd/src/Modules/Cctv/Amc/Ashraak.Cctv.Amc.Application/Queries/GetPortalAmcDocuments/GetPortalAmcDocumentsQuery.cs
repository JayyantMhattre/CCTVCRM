using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPortalAmcDocuments;

public sealed record GetPortalAmcDocumentsQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<IReadOnlyList<AmcContractDocumentDto>>>;
