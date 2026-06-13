using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.CreateAdHocSchedule;

public sealed record CreateAdHocScheduleCommand(
    Guid TenantId,
    Guid UserId,
    Guid ContractTermId,
    Guid SiteId,
    DateOnly ScheduledDate,
    string? Notes) : IRequest<Result<ScheduleDetailDto>>;
