using EssentialFrame.Cqrs.Commands.Store.Models;

namespace EssentialFrame.Cqrs.Commands.Store;

public interface ICommandStore
{
    bool Exists(Guid commandIdentifier);

    Task<bool> ExistsAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    CommandData Get(Guid commandIdentifier);

    Task<CommandData> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    IReadOnlyCollection<CommandData> GetPossibleToSend(DateTimeOffset at);

    Task<IReadOnlyCollection<CommandData>>
        GetPossibleToSendAsync(DateTimeOffset at, CancellationToken cancellationToken = default);

    void Save(CommandData commandData, bool isNew);

    Task SaveAsync(CommandData commandData,
                   bool isNew,
                   CancellationToken cancellationToken = default);
}




