using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetSiteAssetSummary;

public sealed record GetSiteAssetSummaryQuery(
    Guid TenantId,
    Guid UserId,
    Guid SiteId) : IRequest<Result<SiteAssetSummaryDto>>;
