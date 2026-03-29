namespace NikKit.Outbox.Abstraction;

/// <summary>
/// Repo for handling events.
/// </summary>
public interface IOutboxRepository
{
    public string CleanupPolicy { get; }
    public bool UseExternalUnitOfWork { get; }
    
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken ctk = default);
    Task MarkAsCompletedAsync(IEnumerable<Guid> ids, CancellationToken ctk);
    Task MarkAsFailedAsync(IEnumerable<OutboxFailureUpdate> failed, CancellationToken ctk);
}

public record OutboxFailureUpdate(Guid Id, string ErrorMessage, int Jitting, bool IsDeadLetter = false);