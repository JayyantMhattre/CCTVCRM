using Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Events;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Customer;

/// <summary>Customer aggregate root (schema <c>cctv_customer.customers</c>).</summary>
public sealed class Customer : AggregateRoot<CustomerId>
{
    private Customer(CustomerId id) : base(id) { }

    public string CustomerNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string BillingAddress { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public CustomerStatus Status { get; private set; }
    public Guid? PortalUserId { get; private set; }
    public Guid? SourceLeadId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; private set; }

    public static Customer CreateManual(
        CustomerId id,
        string customerNumber,
        string name,
        string email,
        string phone,
        string billingAddress,
        string city,
        Guid createdBy)
    {
        var customer = new Customer(id)
        {
            CustomerNumber = customerNumber,
            Name = name.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            BillingAddress = billingAddress.Trim(),
            City = city.Trim(),
            Status = CustomerStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        customer.RaiseDomainEvent(new CustomerCreatedDomainEvent(
            id.Value, customerNumber, customer.Name, customer.Email, null));

        return customer;
    }

    public static Customer CreateFromLead(
        CustomerId id,
        string customerNumber,
        string name,
        string email,
        string phone,
        string billingAddress,
        string city,
        Guid sourceLeadId,
        Guid createdBy)
    {
        var customer = new Customer(id)
        {
            CustomerNumber = customerNumber,
            Name = name.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            BillingAddress = billingAddress.Trim(),
            City = city.Trim(),
            Status = CustomerStatus.Active,
            SourceLeadId = sourceLeadId,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        customer.RaiseDomainEvent(new CustomerCreatedDomainEvent(
            id.Value, customerNumber, customer.Name, customer.Email, sourceLeadId));

        return customer;
    }

    public void UpdateDetails(
        string name,
        string email,
        string phone,
        string billingAddress,
        string city,
        Guid updatedBy)
    {
        EnsureActive();

        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        BillingAddress = billingAddress.Trim();
        City = city.Trim();
        Touch(updatedBy);

        RaiseDomainEvent(new CustomerUpdatedDomainEvent(Id.Value, CustomerNumber, updatedBy));
    }

    public void ChangeStatus(CustomerStatus toStatus, Guid changedBy)
    {
        if (Status == toStatus)
            return;

        if (toStatus == CustomerStatus.Inactive && Status == CustomerStatus.Active)
        {
            Status = CustomerStatus.Inactive;
            Touch(changedBy);
            RaiseDomainEvent(new CustomerDeactivatedDomainEvent(Id.Value, CustomerNumber, changedBy));
            return;
        }

        if (toStatus == CustomerStatus.Active && Status == CustomerStatus.Inactive)
        {
            Status = CustomerStatus.Active;
            Touch(changedBy);
            RaiseDomainEvent(new CustomerUpdatedDomainEvent(Id.Value, CustomerNumber, changedBy));
            return;
        }

        throw new InvalidOperationException($"Transition from {Status} to {toStatus} is not allowed.");
    }

    public void LinkPortalUser(Guid portalUserId, Guid linkedBy)
    {
        EnsureActive();

        if (PortalUserId.HasValue && PortalUserId.Value != portalUserId)
            throw new InvalidOperationException("Customer already linked to a different portal user.");

        PortalUserId = portalUserId;
        Touch(linkedBy);
        RaiseDomainEvent(new CustomerUpdatedDomainEvent(Id.Value, CustomerNumber, linkedBy));
    }

    public void UpdateOwnProfile(string name, string phone, string email, Guid updatedBy)
    {
        EnsureActive();

        Name = name.Trim();
        Phone = phone.Trim();
        Email = email.Trim();
        Touch(updatedBy);

        RaiseDomainEvent(new CustomerUpdatedDomainEvent(Id.Value, CustomerNumber, updatedBy));
    }

    public void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        Touch(deletedBy);
    }

    private void EnsureActive()
    {
        if (Status == CustomerStatus.Inactive)
            throw new InvalidOperationException("Inactive customers are read-only.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }
}
