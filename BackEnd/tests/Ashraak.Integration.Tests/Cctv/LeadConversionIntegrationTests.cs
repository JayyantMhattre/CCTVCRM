using Ashraak.Cctv.Amc.Application.Abstractions;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.Cctv.Amc.Infrastructure.Services;
using Ashraak.Cctv.Customer.Application.Abstractions;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.Cctv.Customer.Infrastructure.Services;
using Ashraak.Cctv.Lead.Domain.Aggregates.Lead;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.Cctv.Lead.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>Cross-module lead conversion (B1 → B2 → B3) via real provisioning services.</summary>
public sealed class LeadConversionIntegrationTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task ConvertAsync_ProvisionsCustomerSiteAndContractAcrossModules()
    {
        var customerRepository = Substitute.For<ICustomerRepository>();
        var siteRepository = Substitute.For<ISiteRepository>();
        var planRepository = Substitute.For<IAmcPlanRepository>();
        var contractRepository = Substitute.For<IAmcContractRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var customerNumberGenerator = Substitute.For<ICustomerNumberGenerator>();
        customerNumberGenerator.GenerateNextAsync(Arg.Any<CancellationToken>()).Returns("CU-2026-0001");

        var siteNumberGenerator = Substitute.For<ISiteNumberGenerator>();
        siteNumberGenerator.GenerateNextAsync(Arg.Any<CancellationToken>()).Returns("ST-2026-0001");

        var contractNumberGenerator = Substitute.For<IAmcContractNumberGenerator>();
        contractNumberGenerator.GenerateNextAsync(Arg.Any<CancellationToken>()).Returns("AMC-2026-0001");

        var versionId = AmcPlanVersionId.New();
        var plan = CreatePublishedPlan(versionId);
        var publishedVersion = plan.GetVersion(versionId);

        planRepository.GetVersionByIdAsync(versionId, Arg.Any<CancellationToken>()).Returns(publishedVersion);
        contractRepository.GetActiveBySiteIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ContractAggregate?)null);

        var customerProvisioning = new CustomerProvisioningService(
            customerRepository,
            customerNumberGenerator,
            unitOfWork);
        var siteProvisioning = new SiteProvisioningService(
            siteRepository,
            siteNumberGenerator,
            unitOfWork);
        var contractProvisioning = new AmcContractProvisioningService(
            planRepository,
            contractRepository,
            contractNumberGenerator,
            unitOfWork);
        var orchestrator = new LeadConversionOrchestrator(
            customerProvisioning,
            siteProvisioning,
            contractProvisioning);

        var lead = CreateWonLead();
        var request = new ConvertLeadRequest(
            versionId.Value,
            "Main Office",
            "123 Industrial Area",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            lead.RowVersion);

        var result = await orchestrator.ConvertAsync(lead, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().NotBeEmpty();
        result.Value.SiteId.Should().NotBeEmpty();
        result.Value.ContractId.Should().NotBeEmpty();
        result.Value.TermId.Should().NotBeEmpty();

        customerRepository.Received(1).Add(Arg.Any<CustomerAggregate>());
        siteRepository.Received(1).Add(Arg.Any<SiteAggregate>());
        contractRepository.Received(1).Add(Arg.Any<ContractAggregate>());
        await unitOfWork.Received(3).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static PlanAggregate CreatePublishedPlan(AmcPlanVersionId versionId)
    {
        var plan = PlanAggregate.Create(AmcPlanId.New(), "GOLD", "Gold Plan", "Premium coverage", TestUserId);
        plan.AddVersion(
            versionId,
            5000m,
            4,
            "[\"Cleaning\"]",
            "SLA",
            DateOnly.FromDateTime(DateTime.UtcNow),
            TestUserId);
        plan.PublishVersion(versionId, TestUserId);
        return plan;
    }

    private static Lead CreateWonLead()
    {
        var lead = Lead.CreateFromInquiry(
            LeadId.New(),
            "LD-2026-0001",
            LeadSource.WebsiteContact,
            "Jane Doe",
            "Acme Corp",
            "jane@example.com",
            "+919999999999",
            "Mumbai",
            null,
            "CCTV installation",
            TestUserId);

        lead.ChangeStatus(LeadStatus.Contacted, TestUserId, null);
        lead.ChangeStatus(LeadStatus.Qualified, TestUserId, null);
        lead.ChangeStatus(LeadStatus.QuotationSent, TestUserId, null);
        lead.ChangeStatus(LeadStatus.Negotiation, TestUserId, null);
        lead.ChangeStatus(LeadStatus.Won, TestUserId, null);
        return lead;
    }
}
