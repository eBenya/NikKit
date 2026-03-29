using System.Text.Json;

namespace NikKit.Outbox.Abstraction;

public class OutboxMessage
{
    public required Guid Id { get; init; }
    public required string EventType { get; init; }
    public required DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
    public required string PartitionKey { get; init; }

    public required JsonDocument Content { get; init; }

    public EventStatus Status { get; init; }
    public int Attempts { get; init; } = 0;
    public string? LastErrorMessage { get; init; } = null;
    public DateTimeOffset? LastAttemptAt { get; init; } = null;
    public DateTimeOffset? LockUntil { get; init; } = null;
    public string? LockedByNode { get; init; } = null;
}

public enum EventStatus
{
    Unknown = 0,
    Created,
    Processing,
    Failed,
    Cancelled,
    Success
}