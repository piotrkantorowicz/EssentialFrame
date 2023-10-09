using EssentialFrame.Cqrs.Commands.Persistence.Models;

namespace EssentialFrame.Cqrs.Commands.Persistence.Interfaces;

public interface ICommandStore
{
    bool Exists(Guid commandIdentifier);

    Task<bool> ExistsAsync(Guid commandIdentifier, CancellationToken cancellationToken);

    CommandDataModel Get(Guid commandIdentifier);

    Task<CommandDataModel> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken);

    IReadOnlyCollection<CommandDataModel> GetPossibleToSend(DateTimeOffset at);

    Task<IReadOnlyCollection<CommandDataModel>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken);

    void Save(CommandDataModel commandDataModel, bool isNew);

    Task SaveAsync(CommandDataModel commandDataModel, bool isNew, CancellationToken cancellationToken);
}