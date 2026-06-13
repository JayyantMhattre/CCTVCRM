using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateSite;

public sealed record UpdateSiteCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    string Name,
    string Address,
    string City,
    uint RowVersion) : IRequest<Result<SiteDetailDto>>;
