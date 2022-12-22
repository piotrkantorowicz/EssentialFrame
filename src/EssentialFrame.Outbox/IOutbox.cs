namespace EssentialFrame.Outbox;

public interface IOutbox
{
    void Add(OutboxMessage outboxMessage);

    void Save();
}




