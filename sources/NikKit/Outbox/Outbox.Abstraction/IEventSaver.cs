namespace NikKit.Outbox.Abstraction;

/// <summary>
/// Repo for saving events.
/// </summary>
public interface IEventSaver
{
    Task SaveEventsAsync(IEnumerable<IEvent> events, CancellationToken ctk);
}