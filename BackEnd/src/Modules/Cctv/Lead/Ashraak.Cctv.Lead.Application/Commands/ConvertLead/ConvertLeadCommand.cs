using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.ConvertLead;

public sealed record ConvertLeadCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    Guid PlanVersionId,
    string SiteName,
    string SiteAddress,
    DateOnly InitialTermStartDate,
    DateOnly InitialTermEndDate,
    uint RowVersion) : IRequest<Result<LeadConversionResultDto>>;
