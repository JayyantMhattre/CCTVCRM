using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetPortalSiteDetail;

public sealed record GetPortalSiteDetailQuery(
    Guid TenantId,
    Guid UserId,
    Guid SiteId) : IRequest<Result<SiteDetailDto>>;
