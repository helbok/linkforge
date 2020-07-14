namespace LinkForge.Core;

public enum StoreKind
{
    Memory,
    Json
}

public sealed class ShortenerOptions
{
    public StoreKind Store { get; set; } = StoreKind.Memory;

    public string StorePath { get; set; } = string.Empty;
}
