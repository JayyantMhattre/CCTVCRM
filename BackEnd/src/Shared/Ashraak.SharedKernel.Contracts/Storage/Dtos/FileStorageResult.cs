namespace Ashraak.SharedKernel.Contracts.Storage.Dtos;

/// <summary>Result of a successful blob upload.</summary>
public sealed record FileStorageResult(
    string StoragePath,
    long SizeBytes);
