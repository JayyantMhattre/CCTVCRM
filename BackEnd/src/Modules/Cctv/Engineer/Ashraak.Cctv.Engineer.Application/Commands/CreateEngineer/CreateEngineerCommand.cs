using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Commands.CreateEngineer;

public sealed record CreateEngineerCommand(
    Guid TenantId,
    Guid UserId,
    string Name,
    string Phone,
    Guid? PlatformUserId) : IRequest<Result<EngineerDetailDto>>;
