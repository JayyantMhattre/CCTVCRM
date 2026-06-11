namespace Ashraak.SharedKernel.Contracts.Storage.Dtos;

/// <summary>Virus scan outcome for an uploaded file.</summary>
public sealed record FileScanResult(bool IsClean, string? ThreatName = null);
