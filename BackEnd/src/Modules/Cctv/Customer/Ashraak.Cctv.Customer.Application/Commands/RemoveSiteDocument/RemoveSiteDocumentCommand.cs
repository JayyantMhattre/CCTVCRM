using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.RemoveSiteDocument;

public sealed record RemoveSiteDocumentCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    Guid DocumentId,
    uint RowVersion) : IRequest<Result>;
