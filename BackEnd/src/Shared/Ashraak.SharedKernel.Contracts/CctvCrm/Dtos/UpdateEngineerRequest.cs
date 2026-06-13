namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Update engineer (PUT /cctv/engineers/{id}).</summary>
public sealed record UpdateEngineerRequest(
    string Name,
    string Phone,
    Guid? PlatformUserId,
    uint RowVersion);
