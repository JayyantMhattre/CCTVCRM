using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLead;

public sealed record CreateLeadCommand(
    Guid TenantId,
    Guid UserId,
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    string Source,
    Guid? OwnerUserId) : IRequest<Result<LeadDetailDto>>;
