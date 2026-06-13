namespace Ashraak.Cctv.Engineer.Application.Abstractions;

/// <summary>Generates business numbers in format <c>EN-YYYY-NNNN</c>.</summary>
public interface IEngineerNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
