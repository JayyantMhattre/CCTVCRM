namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/schedules/{scheduleId}/assign.</summary>
public sealed record AssignEngineerRequest(
    Guid EngineerId,
    uint RowVersion);
