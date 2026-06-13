using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.SubmitVisitReport;

public sealed record SubmitVisitReportCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    uint RowVersion,
    string? ClientCorrelationId) : IRequest<Result<VisitDetailDto>>;
