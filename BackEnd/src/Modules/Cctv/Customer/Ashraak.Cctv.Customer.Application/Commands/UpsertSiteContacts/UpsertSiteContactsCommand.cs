using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpsertSiteContacts;

public sealed record UpsertSiteContactsCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    IReadOnlyList<SiteContactInputDto> Contacts,
    uint RowVersion) : IRequest<Result<IReadOnlyList<SiteContactDto>>>;
