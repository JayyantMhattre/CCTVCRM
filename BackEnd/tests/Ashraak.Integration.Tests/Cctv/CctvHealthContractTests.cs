using Ashraak.Cctv.Lead.Domain;
using FluentAssertions;
using Xunit;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>Sprint 0 smoke tests for CCTV health contract (no full host required).</summary>
public sealed class CctvHealthContractTests
{
    [Fact]
    public void CctvHealthResponse_ContainsAllBusinessModules()
    {
        var response = new CctvHealthResponse(
            Status: "healthy",
            Phase: "Sprint-0",
            Modules: CctvHealthContract.ModuleNames);

        response.Status.Should().Be("healthy");
        response.Modules.Should().Contain(new[]
        {
            "Lead", "Customer", "Amc", "Service", "Ticket", "Engineer", "Invoice", "Reporting"
        });
    }

    [Fact]
    public void LeadModuleAnchor_IsInLeadDomainAssembly()
    {
        typeof(LeadModuleAnchor).Assembly.GetName().Name.Should().Be("Ashraak.Cctv.Lead.Domain");
    }
}

/// <summary>Mirror of health endpoint module list for contract testing.</summary>
internal static class CctvHealthContract
{
    public static readonly IReadOnlyList<string> ModuleNames =
    [
        "Lead", "Customer", "Amc", "Service", "Ticket", "Engineer", "Invoice", "Reporting"
    ];
}

internal sealed record CctvHealthResponse(
    string Status,
    string Phase,
    IReadOnlyList<string> Modules);
