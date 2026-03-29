namespace NikKit.Outbox.Abstraction;

public interface IEvent
{
    Guid Id { get; }
    string EventName { get; }
}