using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetSiteContacts;

public sealed record GetSiteContactsQuery(
    Guid TenantId,
    Guid UserId,
    Guid SiteId) : IRequest<Result<IReadOnlyList<SiteContactDto>>>;
