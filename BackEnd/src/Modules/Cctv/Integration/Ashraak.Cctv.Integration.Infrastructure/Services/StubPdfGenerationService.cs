using Ashraak.Cctv.Integration.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>Sprint 0 stub — returns minimal PDF header bytes until library is chosen (ADR-CCTV-0002).</summary>
internal sealed class StubPdfGenerationService(ILogger<StubPdfGenerationService> logger) : IPdfGenerationService
{
    private static readonly byte[] MinimalPdfStub = "%PDF-1.4 stub"u8.ToArray();

    public Task<byte[]> GenerateAsync(
        string templateKey,
        object model,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateKey);
        ArgumentNullException.ThrowIfNull(model);
        logger.LogInformation("PDF stub: template {TemplateKey}", templateKey);
        return Task.FromResult(MinimalPdfStub);
    }
}
