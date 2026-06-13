using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetSiteDocuments;

public sealed record GetSiteDocumentsQuery(
    Guid TenantId,
    Guid UserId,
    Guid SiteId) : IRequest<Result<IReadOnlyList<SiteDocumentDto>>>;
