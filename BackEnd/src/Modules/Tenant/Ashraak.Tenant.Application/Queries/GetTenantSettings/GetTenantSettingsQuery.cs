using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Tenant.Application.Queries.GetTenantSettings;

public sealed record GetTenantSettingsQuery(Guid TenantId) : IRequest<Result<TenantSettingsDto>>;
