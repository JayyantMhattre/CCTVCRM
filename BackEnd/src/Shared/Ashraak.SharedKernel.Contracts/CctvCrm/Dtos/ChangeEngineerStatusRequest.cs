namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Activate/deactivate engineer (PATCH /cctv/engineers/{id}/status).</summary>
public sealed record ChangeEngineerStatusRequest(
    string Status,
    uint RowVersion);
