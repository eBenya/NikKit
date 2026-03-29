namespace NikKit.Outbox.Abstraction;

public abstract class BaseOutboxOptions
{
    public static string SectionName { get; protected set; } = "Outbox";

    public string CleanupPolicy { get; init; } = "Delete";
    public bool UseExternalUnitOfWork { get; init; } = true;
}