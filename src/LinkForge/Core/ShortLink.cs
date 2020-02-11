using System;

namespace LinkForge.Core;

public sealed class ShortLink
{
    public string Code { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
