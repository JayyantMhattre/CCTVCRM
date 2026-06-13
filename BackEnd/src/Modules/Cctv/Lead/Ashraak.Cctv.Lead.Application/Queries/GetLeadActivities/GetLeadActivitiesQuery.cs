using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.GetLeadActivities;

public sealed record GetLeadActivitiesQuery(
    Guid TenantId,
    Guid UserId,
    Guid LeadId) : IRequest<Result<IReadOnlyList<LeadActivityDto>>>;
