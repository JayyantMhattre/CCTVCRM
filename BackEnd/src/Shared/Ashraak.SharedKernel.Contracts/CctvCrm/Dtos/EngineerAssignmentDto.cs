namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Engineer assignment history row.</summary>
public sealed record EngineerAssignmentDto(
    Guid Id,
    Guid EngineerId,
    Guid AssignedBy,
    DateTime AssignedAtUtc,
    bool IsActive);
