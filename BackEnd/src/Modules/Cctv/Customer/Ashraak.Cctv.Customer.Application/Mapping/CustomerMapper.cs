using Ashraak.Cctv.Customer.Domain.Enums;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.Cctv.Customer.Application.Mapping;

internal static class CustomerMapper
{
    public static CustomerSummaryDto ToSummary(CustomerAggregate customer) =>
        new(
            customer.Id.Value,
            customer.CustomerNumber,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.City,
            ToStatusString(customer.Status),
            customer.SourceLeadId,
            customer.CreatedAtUtc);

    public static CustomerDetailDto ToDetail(CustomerAggregate customer) =>
        new(
            customer.Id.Value,
            customer.CustomerNumber,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.BillingAddress,
            customer.City,
            ToStatusString(customer.Status),
            customer.PortalUserId,
            customer.SourceLeadId,
            customer.CreatedAtUtc,
            customer.CreatedBy,
            customer.UpdatedAtUtc,
            customer.UpdatedBy,
            customer.RowVersion);

    public static string ToStatusString(CustomerStatus status) => status.ToString();

    public static CustomerStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<CustomerStatus>(value, ignoreCase: false, out var status))
            throw new ArgumentException($"Invalid customer status: {value}", nameof(value));

        return status;
    }

    public static bool TryParseStatus(string? value, out CustomerStatus status)
    {
        status = default;
        return !string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, ignoreCase: false, out status);
    }
}
