using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.UpdateLead;

public sealed record UpdateLeadCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    Guid? OwnerUserId,
    uint RowVersion) : IRequest<Result<LeadDetailDto>>;
