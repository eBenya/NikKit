namespace NikKit.Outbox.Abstraction;

/// <summary>
/// Registry of publishers that linked to eventType
/// </summary>
public interface IPublisherRegistry
{
    IEnumerable<IOutboxPublisher> GetWriters(string eventType, IEvent eventMeta);
}