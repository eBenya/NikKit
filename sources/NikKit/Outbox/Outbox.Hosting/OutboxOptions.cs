using NikKit.Outbox.Abstraction;

namespace NikKit.Outbox.Hosting;

public class OutboxOptions : BaseOutboxOptions
{
    public int BatchSize { get; init; } = 100;
    public int ProcessingTimeoutSeconds { get; init; } = 60;
    public int MaxRetries { get; init; } = 5;
    public int BaseDelayMs { get; init; } = 1000;
    public int MaxJitterMs { get; init; } = 500;
    public Dictionary<string, IEnumerable<(string Protocol, string Address)>> Routes { get; init; } = [];
}