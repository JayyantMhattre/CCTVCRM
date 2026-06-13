using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.ChangeSiteStatus;

public sealed record ChangeSiteStatusCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    string Status,
    uint RowVersion) : IRequest<Result<SiteDetailDto>>;
