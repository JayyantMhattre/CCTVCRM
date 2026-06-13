using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.Cctv.Customer.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>D1-2 domain tests for customer status transitions and number generation.</summary>
public sealed class CustomerDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void ChangeStatus_ActiveToInactive_SucceedsAndIncrementsRowVersion()
    {
        var customer = CreateCustomer();

        customer.ChangeStatus(CustomerStatus.Inactive, TestUserId);

        customer.Status.Should().Be(CustomerStatus.Inactive);
        customer.RowVersion.Should().Be(2);
    }

    [Fact]
    public void ChangeStatus_InactiveToActive_SucceedsAndIncrementsRowVersion()
    {
        var customer = CreateCustomer();
        customer.ChangeStatus(CustomerStatus.Inactive, TestUserId);

        customer.ChangeStatus(CustomerStatus.Active, TestUserId);

        customer.Status.Should().Be(CustomerStatus.Active);
        customer.RowVersion.Should().Be(3);
    }

    [Fact]
    public void UpdateOwnProfile_UpdatesAllowedFieldsOnly()
    {
        var customer = CreateCustomer();
        var originalAddress = customer.BillingAddress;

        customer.UpdateOwnProfile("New Name", "+911234567890", "new@example.com", TestUserId);

        customer.Name.Should().Be("New Name");
        customer.Phone.Should().Be("+911234567890");
        customer.Email.Should().Be("new@example.com");
        customer.BillingAddress.Should().Be(originalAddress);
    }

    [Fact]
    public void CreateFromLead_SetsSourceLeadId()
    {
        var leadId = Guid.NewGuid();

        var customer = CustomerAggregate.CreateFromLead(
            CustomerId.New(),
            "CU-2026-0001",
            "Acme Corp",
            "info@acme.com",
            "+919999999999",
            "123 Main St",
            "Mumbai",
            leadId,
            TestUserId);

        customer.SourceLeadId.Should().Be(leadId);
        customer.Status.Should().Be(CustomerStatus.Active);
    }

    [Fact]
    public async Task CustomerNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<ICustomerRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(5);

        var generator = new CustomerNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("CU-2026-0006");
        await repository.Received(1).GetYearlySequenceAsync(DateTime.UtcNow.Year, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CustomerNumberGenerator_FirstOfYear_UsesSequenceOne()
    {
        var repository = Substitute.For<ICustomerRepository>();
        repository.GetYearlySequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(0);

        var generator = new CustomerNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().MatchRegex(@"^CU-\d{4}-0001$");
    }

    private static CustomerAggregate CreateCustomer() =>
        CustomerAggregate.CreateManual(
            CustomerId.New(),
            "CU-2026-0001",
            "Jane Doe",
            "jane@example.com",
            "+919999999999",
            "123 Billing St",
            "Mumbai",
            TestUserId);
}
