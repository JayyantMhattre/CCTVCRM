using Ashraak.Cctv.Amc.Domain;
using Ashraak.Cctv.Customer.Domain;
using Ashraak.Cctv.Engineer.Domain;
using Ashraak.Cctv.Integration.Application.Abstractions;
using Ashraak.Cctv.Invoice.Domain;
using Ashraak.Cctv.Lead.Domain;
using Ashraak.Cctv.Reporting.Domain;
using Ashraak.Cctv.Service.Domain;
using Ashraak.Cctv.Ticket.Domain;
using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;
using Xunit;

namespace Ashraak.Architecture.Tests;

/// <summary>Layer and boundary rules for Aarvii CCTV business modules (Sprint 0).</summary>
public sealed class CctvArchitectureTests
{
    private static readonly Type[] DomainAnchors =
    [
        typeof(LeadModuleAnchor),
        typeof(CustomerModuleAnchor),
        typeof(AmcModuleAnchor),
        typeof(ServiceModuleAnchor),
        typeof(TicketModuleAnchor),
        typeof(EngineerModuleAnchor),
        typeof(InvoiceModuleAnchor),
        typeof(ReportingModuleAnchor),
    ];

    [Theory]
    [MemberData(nameof(DomainAnchorData))]
    public void CctvDomainLayer_ShouldNotReferenceApplicationOrInfrastructure(Type anchorType)
    {
        var assembly = anchorType.Assembly;
        var forbiddenSuffixes = new[] { ".Application", ".Infrastructure", ".Api" };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenSuffixes)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{assembly.GetName().Name} Domain must not depend on Application, Infrastructure, or Api layers.");
    }

    [Fact]
    public void CctvLeadDomain_ShouldNotReferenceOtherCctvModuleDomains()
    {
        var forbidden = DomainAnchors
            .Where(t => t != typeof(LeadModuleAnchor))
            .Select(t => t.Assembly.GetName().Name!)
            .ToArray();

        var result = Types.InAssembly(typeof(LeadModuleAnchor).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "CCTV modules must integrate via SharedKernel.Contracts only — not direct domain references.");
    }

    [Fact]
    public void CctvIntegrationApplication_ShouldNotReferenceCctvInfrastructure()
    {
        var forbidden = DomainAnchors
            .Select(t => t.Assembly.GetName().Name!.Replace(".Domain", ".Infrastructure"))
            .ToArray();

        var assembly = typeof(ISmsProvider).Assembly;

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Integration.Application must not reference CCTV module infrastructure projects.");
    }

    public static IEnumerable<object[]> DomainAnchorData() =>
        DomainAnchors.Select(t => new object[] { t });
}
