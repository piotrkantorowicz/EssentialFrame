using EssentialFrame.Cqrs.Commands.Store.Models;

namespace EssentialFrame.Cqrs.Commands.Store.Interfaces;

public interface ICommandStore
{
    bool Exists(Guid commandIdentifier);

    Task<bool> ExistsAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    CommandDao Get(Guid commandIdentifier);

    Task<CommandDao> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    IReadOnlyCollection<CommandDao> GetPossibleToSend(DateTimeOffset at);

    Task<IReadOnlyCollection<CommandDao>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default);

    void Save(CommandDao commandDao, bool isNew);

    Task SaveAsync(CommandDao commandDao, bool isNew, CancellationToken cancellationToken = default);
}