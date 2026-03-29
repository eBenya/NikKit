namespace NikKit.Outbox.Abstraction;

public interface IOutboxPublisher
{
    public string CleanupPolicy { get; }
    public bool UseExternalUnitOfWork { get; }
    
    Task PublishAsync(string payload, string eventType, IEvent eventMeta, CancellationToken ct = default);
}