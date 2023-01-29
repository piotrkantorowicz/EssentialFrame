using EssentialFrame.Cache.Interfaces;
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
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public CommandDao Get(Guid commandIdentifier)
    {
        throw new NotImplementedException();
    }

    public Task<CommandDao> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<CommandDao> GetPossibleToSend(DateTimeOffset at)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<CommandDao>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Save(CommandDao commandDao, bool isNew)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(CommandDao commandDao, bool isNew, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}