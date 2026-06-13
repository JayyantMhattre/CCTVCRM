using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.ApproveVisit;

public sealed record ApproveVisitCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    string? ReviewRemarks) : IRequest<Result<VisitDetailDto>>;
