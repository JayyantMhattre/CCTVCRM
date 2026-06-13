using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLeadActivity;

public sealed record CreateLeadActivityCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    string ActivityType,
    string Description,
    string? FromStatus,
    string? ToStatus) : IRequest<Result<LeadActivityDto>>;
