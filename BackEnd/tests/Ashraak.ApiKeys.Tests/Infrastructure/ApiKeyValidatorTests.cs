using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using Ashraak.ApiKeys.Infrastructure.Persistence;
using Ashraak.ApiKeys.Infrastructure.Persistence.Repositories;
using Ashraak.ApiKeys.Infrastructure.Security;
using Ashraak.ApiKeys.Infrastructure.Services;
using Ashraak.SharedKernel.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Ashraak.ApiKeys.Tests.Infrastructure;

public sealed class ApiKeyValidatorTests
{
    [Fact]
    public async Task ValidateAsync_returns_scopes_for_valid_key()
    {
        var hasher = new ApiKeyHasher();
        var generator = new ApiKeyGenerator();
        var (plaintext, prefix) = generator.Generate("prod");

        var tenantContext = Substitute.For<ITenantContext>();
        tenantContext.TenantId.Returns(Guid.NewGuid());

        var options = new DbContextOptionsBuilder<ApiKeysDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new ApiKeysDbContext(options, tenantContext);
        var repository = new ApiKeyRepository(db);

        var apiKey = ApiKey.Create(
            ApiKeyId.New(),
            Guid.NewGuid(),
            "Validator Test",
            string.Empty,
            prefix,
            hasher.Hash(plaintext),
            "prod",
            ["users:read"],
            Guid.NewGuid(),
            null);
        repository.Add(apiKey);
        await db.SaveChangesAsync();

        var validator = new ApiKeyValidator(repository, hasher);
        var result = await validator.ValidateAsync(plaintext, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Scopes.Should().Contain("users:read");
    }

    [Fact]
    public async Task ValidateAsync_rejects_revoked_key()
    {
        var hasher = new ApiKeyHasher();
        var generator = new ApiKeyGenerator();
        var (plaintext, prefix) = generator.Generate("prod");

        var tenantContext = Substitute.For<ITenantContext>();
        tenantContext.TenantId.Returns(Guid.NewGuid());

        var options = new DbContextOptionsBuilder<ApiKeysDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new ApiKeysDbContext(options, tenantContext);
        var repository = new ApiKeyRepository(db);

        var apiKey = ApiKey.Create(
            ApiKeyId.New(),
            Guid.NewGuid(),
            "Revoked",
            string.Empty,
            prefix,
            hasher.Hash(plaintext),
            "prod",
            ["users:read"],
            Guid.NewGuid(),
            null);
        apiKey.Revoke(Guid.NewGuid());
        repository.Add(apiKey);
        await db.SaveChangesAsync();

        var validator = new ApiKeyValidator(repository, hasher);
        var result = await validator.ValidateAsync(plaintext, CancellationToken.None);

        result.Should().BeNull();
    }
}
