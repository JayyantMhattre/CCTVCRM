using System.Net.Http.Json;
using System.Text.Json;
using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Integration.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>
/// Configuration-driven SMS provider. Supports console logging (default) and generic HTTP gateways.
/// </summary>
internal sealed class ConfiguredSmsProvider(
    IHttpClientFactory httpClientFactory,
    IOptions<SmsOptions> options,
    ILogger<ConfiguredSmsProvider> logger) : ISmsProvider
{
    public async Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        var config = options.Value;
        if (string.Equals(config.Provider, "http", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(config.Http.Endpoint))
        {
            await SendHttpAsync(config, phoneNumber, message, cancellationToken);
            return;
        }

        logger.LogInformation(
            "SMS ({Provider}): to {PhoneNumber} — {MessagePreview}",
            config.Provider,
            phoneNumber,
            message.Length > 120 ? message[..120] + "…" : message);
    }

    private async Task SendHttpAsync(
        SmsOptions config,
        string phoneNumber,
        string message,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("CctvSms");
        using var request = new HttpRequestMessage(HttpMethod.Post, config.Http.Endpoint);
        if (!string.IsNullOrWhiteSpace(config.Http.ApiKeyValue))
            request.Headers.TryAddWithoutValidation(config.Http.ApiKeyHeader, config.Http.ApiKeyValue);

        var payload = new Dictionary<string, string>
        {
            [config.Http.PhoneNumberField] = phoneNumber,
            [config.Http.MessageField] = message
        };

        request.Content = JsonContent.Create(payload);
        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError(
                "SMS HTTP provider failed: {StatusCode} {Body}",
                (int)response.StatusCode,
                body);
            throw new InvalidOperationException($"SMS provider returned {(int)response.StatusCode}");
        }

        logger.LogInformation("SMS sent via HTTP provider to {PhoneNumber}", phoneNumber);
    }
}
