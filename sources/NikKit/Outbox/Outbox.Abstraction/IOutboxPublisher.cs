namespace NikKit.Outbox.Abstraction;

public interface IOutboxPublisher
{
    Task PublishAsync(string payload, string eventType, IEvent eventMeta, CancellationToken ct = default);
}