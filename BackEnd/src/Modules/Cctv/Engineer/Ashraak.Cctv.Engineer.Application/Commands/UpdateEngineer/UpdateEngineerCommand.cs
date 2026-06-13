using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Commands.UpdateEngineer;

public sealed record UpdateEngineerCommand(
    Guid TenantId,
    Guid UserId,
    Guid EngineerId,
    string Name,
    string Phone,
    Guid? PlatformUserId,
    uint RowVersion) : IRequest<Result<EngineerDetailDto>>;
