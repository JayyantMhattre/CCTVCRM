namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Admin customer update (PUT /cctv/customers/{id}).</summary>
public sealed record UpdateCustomerRequest(
    string Name,
    string Email,
    string Phone,
    string BillingAddress,
    string City,
    uint RowVersion);
