using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.UpdateVisitRemarks;

public sealed record UpdateVisitRemarksCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    string Remarks,
    uint RowVersion) : IRequest<Result<VisitDetailDto>>;
