using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateInquiry;

public sealed record CreateInquiryCommand(
    Guid TenantId,
    Guid UserId,
    string InquiryType,
    string Name,
    string? Organization,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    string? PreferredPlanCode,
    string? SourcePage) : IRequest<Result<CreateInquiryResultDto>>;
