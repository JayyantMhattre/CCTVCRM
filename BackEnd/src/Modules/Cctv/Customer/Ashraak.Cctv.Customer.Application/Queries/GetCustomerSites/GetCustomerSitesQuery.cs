using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetCustomerSites;

/// <summary>Returns sites for a customer.</summary>
public sealed record GetCustomerSitesQuery(
    Guid TenantId,
    Guid UserId,
    Guid CustomerId) : IRequest<Result<IReadOnlyList<SiteSummaryDto>>>;
