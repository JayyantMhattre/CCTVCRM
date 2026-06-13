using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.GetLead;

public sealed record GetLeadQuery(
    Guid TenantId,
    Guid UserId,
    Guid LeadId) : IRequest<Result<LeadDetailDto>>;
