using EssentialFrame.Cqrs.CommandStore.Models;

namespace EssentialFrame.Cqrs.CommandStore;

public interface ICommandStore
{
    bool Exists(Guid commandIdentifier);

    Task<bool> ExistsAsync(Guid commandIdentifier);

    CommandData Get(Guid commandIdentifier);

    Task<CommandData> GetAsync(Guid commandIdentifier);

    IEnumerable<CommandData> GetExpired(DateTimeOffset at);

    Task<IEnumerable<CommandData>> GetExpiredAsync(DateTimeOffset at);

    void Save(CommandData command, bool isNew);

    Task SaveAsync(CommandData command, bool isNew);
}
