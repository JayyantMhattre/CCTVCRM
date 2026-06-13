using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLeadRemark;

public sealed record CreateLeadRemarkCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    string Text) : IRequest<Result<LeadRemarkDto>>;
