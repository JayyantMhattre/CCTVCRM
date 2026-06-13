namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Nested line item for create/update invoice requests.</summary>
public sealed record InvoiceLineRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal? TaxRate = null,
    int SortOrder = 0);
