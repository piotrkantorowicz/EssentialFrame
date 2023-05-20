using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Const;
using EssentialFrame.Cqrs.Commands.Store.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Models;

namespace EssentialFrame.Cqrs.Commands.Store;

internal sealed class DefaultCommandStore : ICommandStore
{
    private readonly ICache<Guid, CommandDao> _commandsCache;

    public DefaultCommandStore(ICache<Guid, CommandDao> commandsCache)
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

    public CommandDao Get(Guid commandIdentifier)
    {
        return _commandsCache[commandIdentifier];
    }

    public async Task<CommandDao> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(commandIdentifier));
    }

    public IReadOnlyCollection<CommandDao> GetPossibleToSend(DateTimeOffset at)
    {
        return _commandsCache.GetMany((_, v) =>
            v.SendStatus == CommandSendingStatuses.Scheduled && v.SendScheduled <= at);
    }

    public async Task<IReadOnlyCollection<CommandDao>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetPossibleToSend(at));
    }

    public void Save(CommandDao commandDao, bool isNew)
    {
        _commandsCache.Add(commandDao.CommandIdentifier, commandDao);
    }

    public async Task SaveAsync(CommandDao commandDao, bool isNew, CancellationToken cancellationToken = default)
    {
        Save(commandDao, isNew);

        await Task.CompletedTask;
    }
}