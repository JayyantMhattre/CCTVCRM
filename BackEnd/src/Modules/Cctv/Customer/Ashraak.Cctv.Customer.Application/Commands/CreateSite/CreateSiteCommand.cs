using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.CreateSite;

public sealed record CreateSiteCommand(
    Guid TenantId,
    Guid UserId,
    Guid CustomerId,
    string Name,
    string Address,
    string City) : IRequest<Result<SiteDetailDto>>;
