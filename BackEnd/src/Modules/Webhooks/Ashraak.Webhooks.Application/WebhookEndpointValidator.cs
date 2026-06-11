using Ashraak.SharedKernel.Results;

namespace Ashraak.Webhooks.Application;

internal static class WebhookEndpointValidator
{
    public static Error? ValidateUrl(string endpointUrl, bool requireHttps)
    {
        if (string.IsNullOrWhiteSpace(endpointUrl))
            return Error.Validation("Webhooks.EndpointRequired", "Endpoint URL is required.");

        if (!Uri.TryCreate(endpointUrl.Trim(), UriKind.Absolute, out var uri))
            return Error.Validation("Webhooks.EndpointInvalid", "Endpoint URL must be a valid absolute URI.");

        if (uri.Scheme is not ("https" or "http"))
            return Error.Validation("Webhooks.EndpointScheme", "Endpoint URL must use HTTP or HTTPS.");

        if (requireHttps && uri.Scheme != Uri.UriSchemeHttps)
            return Error.Validation("Webhooks.EndpointHttps", "Endpoint URL must use HTTPS.");

        if (string.IsNullOrWhiteSpace(uri.Host))
            return Error.Validation("Webhooks.EndpointHost", "Endpoint URL must include a host.");

        return null;
    }
}
