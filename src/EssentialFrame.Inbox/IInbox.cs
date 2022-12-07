namespace EssentialFrame.Inbox;

public interface IInbox
{
    void Add(InboxMessage outboxMessage);

    void Save();
}
