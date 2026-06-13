namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Invoice line item row.</summary>
public sealed record InvoiceLineDto(
    Guid Id,
    int LineNo,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);
