using System;
using LinkForge.Core;
using Xunit;

namespace LinkForge.Tests;

public class UrlValidatorTests
{
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path?q=1")]
    public void Validate_accepts_http_and_https(string url)
    {
        UrlValidator.Validate(url);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ftp://example.com")]
    [InlineData("javascript:alert(1)")]
    [InlineData("example.com")]
    public void Validate_rejects_non_http_urls(string url)
    {
        Assert.Throws<ArgumentException>(() => UrlValidator.Validate(url));
    }
}
