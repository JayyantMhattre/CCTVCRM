using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.GetLeadRemarks;

public sealed record GetLeadRemarksQuery(
    Guid TenantId,
    Guid UserId,
    Guid LeadId) : IRequest<Result<IReadOnlyList<LeadRemarkDto>>>;
