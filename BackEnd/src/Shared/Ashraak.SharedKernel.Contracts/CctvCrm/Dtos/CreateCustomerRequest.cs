namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Manual customer creation (POST /cctv/customers).</summary>
public sealed record CreateCustomerRequest(
    string Name,
    string Email,
    string Phone,
    string BillingAddress,
    string City);
