using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Const;
using EssentialFrame.Cqrs.Commands.Store.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Models;

namespace EssentialFrame.Cqrs.Commands.Store;

internal sealed class DefaultCommandStore : ICommandStore
{
    private readonly ICache<Guid, CommandDataModel> _commandsCache;

    public DefaultCommandStore(ICache<Guid, CommandDataModel> commandsCache)
    {
        _commandsCache = commandsCache ?? throw new ArgumentNullException(nameof(commandsCache));
    }

    public bool Exists(Guid commandIdentifier)
    {
        return _commandsCache.Exists(commandIdentifier);
    }

    public async Task<bool> ExistsAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Exists(commandIdentifier));
    }

    public CommandDataModel Get(Guid commandIdentifier)
    {
        return _commandsCache[commandIdentifier];
    }

    public async Task<CommandDataModel> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(commandIdentifier));
    }

    public IReadOnlyCollection<CommandDataModel> GetPossibleToSend(DateTimeOffset at)
    {
        return _commandsCache.GetMany((_, v) =>
            v.SendStatus == CommandSendingStatuses.Scheduled && v.SendScheduled <= at);
    }

    public async Task<IReadOnlyCollection<CommandDataModel>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetPossibleToSend(at));
    }

    public void Save(CommandDataModel commandDataModel, bool isNew)
    {
        _commandsCache.Add(commandDataModel.CommandIdentifier, commandDataModel);
    }

    public async Task SaveAsync(CommandDataModel commandDataModel, bool isNew,
        CancellationToken cancellationToken = default)
    {
        Save(commandDataModel, isNew);

        await Task.CompletedTask;
    }
}