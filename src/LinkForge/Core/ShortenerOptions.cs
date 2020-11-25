namespace LinkForge.Core;

public enum StoreKind
{
    Memory,
    Json
}

public sealed class ShortenerOptions
{
    public const int DefaultMaxUrlLength = 2048;

    public StoreKind Store { get; set; } = StoreKind.Memory;

    public string StorePath { get; set; } = string.Empty;

    public int MaxUrlLength { get; set; } = DefaultMaxUrlLength;

    public string BaseUrl { get; set; } = string.Empty;
}
