using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.CaptureVisitLocation;

public sealed record CaptureVisitLocationCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    decimal Latitude,
    decimal Longitude,
    DateTime CapturedAt) : IRequest<Result<VisitDetailDto>>;
