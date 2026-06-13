using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetPortalSites;

public sealed record GetPortalSitesQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<IReadOnlyList<SiteSummaryDto>>>;
