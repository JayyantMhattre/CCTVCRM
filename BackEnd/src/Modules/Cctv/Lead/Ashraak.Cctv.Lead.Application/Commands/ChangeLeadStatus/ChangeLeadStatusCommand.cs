using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.ChangeLeadStatus;

public sealed record ChangeLeadStatusCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    string ToStatus,
    string? Notes,
    uint RowVersion) : IRequest<Result<LeadDetailDto>>;
