using System;

namespace LinkForge.Core;

public static class UrlValidator
{
    public static void Validate(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL must not be empty.", nameof(url));
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException(
                "Only absolute http:// or https:// URLs are supported.", nameof(url));
        }
    }
}
