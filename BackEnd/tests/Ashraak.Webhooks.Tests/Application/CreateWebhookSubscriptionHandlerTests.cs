using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Application.Commands.CreateWebhookSubscription;
using Ashraak.Webhooks.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Ashraak.Webhooks.Tests.Application;

public sealed class CreateWebhookSubscriptionHandlerTests
{
    private readonly IWebhookSubscriptionStore _store = Substitute.For<IWebhookSubscriptionStore>();
    private readonly IAuthPermissionChecker _permissions = Substitute.For<IAuthPermissionChecker>();
    private readonly IFeatureFlagService _featureFlags = Substitute.For<IFeatureFlagService>();
    private readonly IWebhookSecretGenerator _secretGenerator = Substitute.For<IWebhookSecretGenerator>();
    private readonly IWebhookSecretProtector _secretProtector = Substitute.For<IWebhookSecretProtector>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IOptions<WebhookOptions> _options = Options.Create(new WebhookOptions { RequireHttpsEndpoints = false });

    public CreateWebhookSubscriptionHandlerTests()
    {
        _featureFlags.IsEnabledAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(true);
        _permissions.HasPermissionAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), "webhooks:manage", Arg.Any<CancellationToken>())
            .Returns(true);
        _secretGenerator.Generate().Returns("plain-secret");
        _secretProtector.Protect("plain-secret").Returns("protected-secret");
        _store.ExistsByNameAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
    }

    [Fact]
    public async Task Handle_WithoutManagePermission_ReturnsForbidden()
    {
        _permissions.HasPermissionAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), "webhooks:manage", Arg.Any<CancellationToken>())
            .Returns(false);
        _permissions.IsInRoleAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), "Admin", Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = new CreateWebhookSubscriptionCommandHandler(
            _store, _permissions, _featureFlags, _secretGenerator, _secretProtector, _options, _unitOfWork);

        var result = await handler.Handle(
            new CreateWebhookSubscriptionCommand(Guid.NewGuid(), Guid.NewGuid(), "Hook", "https://a.test/hook"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Webhooks.ManageForbidden");
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesSubscription()
    {
        var handler = new CreateWebhookSubscriptionCommandHandler(
            _store, _permissions, _featureFlags, _secretGenerator, _secretProtector, _options, _unitOfWork);

        var tenantId = Guid.NewGuid();
        var result = await handler.Handle(
            new CreateWebhookSubscriptionCommand(tenantId, Guid.NewGuid(), "Hook", "https://a.test/hook"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Secret.Should().Be("plain-secret");
        _store.Received(1).Add(Arg.Any<Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription.WebhookSubscription>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFeatureDisabled_ReturnsForbidden()
    {
        _featureFlags.IsEnabledAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = new CreateWebhookSubscriptionCommandHandler(
            _store, _permissions, _featureFlags, _secretGenerator, _secretProtector, _options, _unitOfWork);

        var result = await handler.Handle(
            new CreateWebhookSubscriptionCommand(Guid.NewGuid(), Guid.NewGuid(), "Hook", "https://a.test/hook"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Webhooks.Disabled");
    }
}
