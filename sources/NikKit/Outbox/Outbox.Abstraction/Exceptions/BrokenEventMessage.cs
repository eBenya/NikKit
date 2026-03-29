using NikKit.DomainExceptions;

namespace NikKit.Outbox.Abstraction.Exceptions;

public class BrokenEventMessage: DomainException
{
    public BrokenEventMessage(OutboxMessage message) 
        : base($"Event message with ID {message.Id} has broken content.")
    {
        AddData($"{nameof(OutboxMessage)}-{nameof(OutboxMessage.Id)}", message.Id);
        AddData($"{nameof(OutboxMessage)}-{nameof(OutboxMessage.Content)}", message.Content.RootElement.GetRawText());
    }
}