using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetSite;

public sealed record GetSiteQuery(
    Guid TenantId,
    Guid UserId,
    Guid SiteId) : IRequest<Result<SiteDetailDto>>;
