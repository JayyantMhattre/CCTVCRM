namespace Ashraak.Cctv.Lead.Application.Abstractions;

/// <summary>Generates unique lead business numbers (<c>LD-YYYY-NNNN</c>).</summary>
public interface ILeadNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
