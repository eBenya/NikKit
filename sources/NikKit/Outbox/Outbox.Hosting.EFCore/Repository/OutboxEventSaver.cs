using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NikKit.IEnumerableExtensions;
using NikKit.Outbox.Abstraction;

namespace NikKit.Outbox.Hosting.EFCore.Repository;

public class OutboxEventSaver(DbContext dbContext): IEventSaver
{
    public virtual async Task SaveEventsAsync(IEnumerable<IEvent> events, CancellationToken ctk)
    {
        var domainEvents = events.AsList();
        if (domainEvents.Count == 0) return;
        var now = DateTimeOffset.UtcNow;

        var messages = domainEvents.Select(@event =>
        {
            var id = Guid.CreateVersion7();
            var content = JsonSerializer.SerializeToDocument(@event, @event.GetType());
            var eventType = @event.GetType().Name;
            return new OutboxMessage
            {
                Id = id,
                EventType = eventType,
                Content = content,
                Created = now.UtcDateTime,
                PartitionKey = @event.EventName,
            };
        });

        await dbContext.Set<OutboxMessage>().AddRangeAsync(messages, ctk);
    }
}