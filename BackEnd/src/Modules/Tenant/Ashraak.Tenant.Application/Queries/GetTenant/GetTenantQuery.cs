using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Tenant.Application.Queries.GetTenant;

/// <summary>
/// Query to retrieve a tenant's public-facing data by its identifier.
/// Returns <see cref="TenantDto"/> on success, or <c>404 Not Found</c> if no such tenant exists.
/// </summary>
/// <param name="TenantId">The GUID of the tenant to retrieve.</param>
public sealed record GetTenantQuery(Guid TenantId) : IRequest<Result<TenantDto>>;
