namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Activate/deactivate customer (PATCH /cctv/customers/{id}/status).</summary>
public sealed record ChangeCustomerStatusRequest(
    string Status,
    uint RowVersion);
