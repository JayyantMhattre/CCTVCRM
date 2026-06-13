using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateSiteAssetSummary;

public sealed record UpdateSiteAssetSummaryCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    int CameraCount,
    int DvrCount,
    int NvrCount,
    int HardDiskCount,
    int SwitchCount,
    int RouterCount,
    int MonitorCount,
    string? Brand,
    string? Model,
    string? Remarks,
    uint RowVersion) : IRequest<Result<SiteAssetSummaryDto>>;
