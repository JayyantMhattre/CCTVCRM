using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.OfflineVisitSyncBatch;

public sealed record OfflineVisitSyncBatchCommand(
    Guid TenantId,
    Guid UserId,
    IReadOnlyList<OfflineVisitSyncItemRequest> Items) : IRequest<Result<OfflineSyncResultDto>>;
