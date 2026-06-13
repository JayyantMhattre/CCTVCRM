using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Commands.ChangeEngineerStatus;

public sealed record ChangeEngineerStatusCommand(
    Guid TenantId,
    Guid UserId,
    Guid EngineerId,
    string Status,
    uint RowVersion) : IRequest<Result<EngineerDetailDto>>;
