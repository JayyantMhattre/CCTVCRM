namespace Ashraak.Cctv.Integration.Application.Abstractions;

/// <summary>
/// Server-side PDF generation for AMC contracts, visit reports, and invoices.
/// Sprint 0: stub returns empty bytes — library selected via ADR.
/// </summary>
public interface IPdfGenerationService
{
    /// <summary>Generates a PDF document from the supplied template key and model payload (QuestPDF).</summary>
    Task<byte[]> GenerateAsync(
        string templateKey,
        object model,
        CancellationToken cancellationToken = default);
}
