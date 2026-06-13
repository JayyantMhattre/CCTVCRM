using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.LinkSiteDocument;

public sealed record LinkSiteDocumentCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    Guid FileId,
    string DocumentType,
    string Title,
    uint RowVersion) : IRequest<Result<SiteDocumentDto>>;
