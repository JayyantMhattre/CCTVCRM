using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Ashraak.Architecture.Tests;

public sealed class ArchitectureTests
{
    private static readonly string[] ModuleNames = ["Auth", "Tenant", "Users", "Audit", "Caching", "Notifications", "Files", "Webhooks"];

    // ─── SharedKernel Boundary Rules ─────────────────────────────────────────

    [Fact]
    public void SharedKernel_ShouldNotReferenceAnyModule()
    {
        var result = Types.InAssembly(typeof(SharedKernel.Domain.Primitives.Entity<>).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ModuleNames.Select(m => $"Ashraak.{m}").ToArray())
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"SharedKernel must not depend on any module. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void SharedKernelContracts_ShouldNotReferenceModuleImplementations()
    {
        var forbidden = ModuleNames
            .SelectMany(m => new[]
            {
                $"Ashraak.{m}.Domain",
                $"Ashraak.{m}.Application",
                $"Ashraak.{m}.Infrastructure"
            })
            .ToArray();

        var result = Types.InAssembly(typeof(SharedKernel.Contracts.Tenant.Dtos.TenantDto).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"SharedKernel.Contracts must not depend on module implementations. Violations: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    // ─── Domain Layer Rules ───────────────────────────────────────────────────

    [Theory]
    [InlineData("Ashraak.Auth.Domain")]
    [InlineData("Ashraak.Tenant.Domain")]
    [InlineData("Ashraak.Users.Domain")]
    [InlineData("Ashraak.Audit.Domain")]
    public void DomainLayer_ShouldNotReferenceApplicationOrInfrastructure(string assemblyName)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        if (assembly is null)
            return;

        var forbiddenSuffixes = new[] { ".Application", ".Infrastructure", ".Api" };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenSuffixes)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{assemblyName} Domain must not depend on Application or Infrastructure layers.");
    }

    // ─── Cross-Module Rules ───────────────────────────────────────────────────

    [Fact]
    public void AuthModule_ShouldNotDirectlyReferenceUsersDomain()
    {
        var authAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith("Ashraak.Auth") == true);

        foreach (var assembly in authAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOn("Ashraak.Users.Domain")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Auth module must not directly reference Users.Domain in {assembly.GetName().Name}");
        }
    }

    [Fact]
    public void TenantModule_ShouldNotDirectlyReferenceAuthDomain()
    {
        var tenantAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith("Ashraak.Tenant") == true);

        foreach (var assembly in tenantAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOn("Ashraak.Auth.Domain")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Tenant module must not directly reference Auth.Domain in {assembly.GetName().Name}");
        }
    }

    // ─── Naming Convention Rules ──────────────────────────────────────────────

    [Fact]
    public void CommandHandlers_ShouldBeInternal()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.Contains(".Application") == true);

        foreach (var assembly in assemblies)
        {
            var result = Types.InAssembly(assembly)
                .That().HaveNameEndingWith("CommandHandler")
                .Should().NotBePublic()
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"CommandHandlers in {assembly.GetName().Name} should be internal (not public API).");
        }
    }

    [Fact]
    public void Repositories_ShouldBeInternal()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.Contains(".Infrastructure") == true);

        foreach (var assembly in assemblies)
        {
            var result = Types.InAssembly(assembly)
                .That().HaveNameEndingWith("Repository")
                .Should().NotBePublic()
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"Repository implementations in {assembly.GetName().Name} should be internal.");
        }
    }
}
