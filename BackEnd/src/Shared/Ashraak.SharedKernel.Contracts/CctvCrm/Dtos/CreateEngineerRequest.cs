namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Create engineer (POST /cctv/engineers).</summary>
public sealed record CreateEngineerRequest(
    string Name,
    string Phone,
    Guid? PlatformUserId);
