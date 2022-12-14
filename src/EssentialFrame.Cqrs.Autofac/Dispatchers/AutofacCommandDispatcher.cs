using Autofac;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Autofac.Exceptions;
using EssentialFrame.Cqrs.Commands.Store;
using EssentialFrame.Cqrs.Dispatchers;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs.Autofac.Dispatchers;

internal sealed class AutofacCommandDispatcher : CommandDispatcherBase
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacCommandDispatcher(ICommandStore commandStore, ILifetimeScope lifetimeScope)
        : base(commandStore) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public override ICommandResult Send<TCommand>(TCommand command)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        return base.Send(command);
    }

    public override void CancelSending<TCommand>(TCommand command)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        base.CancelSending(command);
    }

    public override async Task CancelSendingAsync<TCommand>(TCommand command,
                                                            CancellationToken cancellationToken = default)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        await base.CancelSendingAsync(command, cancellationToken);
    }

    public override async Task<ICommandResult> SendAsync<TCommand>(TCommand command,
                                                                   CancellationToken cancellationToken = default)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        return await base.SendAsync(command, cancellationToken);
    }

    public override ICommandResult SendAndStore<TCommand>(TCommand command)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        return base.SendAndStore(command);
    }

    public override async Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command,
                                                                           CancellationToken cancellationToken =
                                                                               default)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        return await base.SendAndStoreAsync(command, cancellationToken);
    }

    public override void Schedule<T>(T command, DateTimeOffset at)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        base.Schedule(command, at);
    }

    public override async Task ScheduleAsync<T>(T command,
                                                DateTimeOffset at,
                                                CancellationToken cancellationToken = default)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        await base.ScheduleAsync(command,
                                 at,
                                 cancellationToken);
    }

    protected override THandler FindHandler<TCommand, THandler>(TCommand command)
    {
        var isTenantHandlerFound =
            _lifetimeScope.TryResolveKeyed(command.TenantIdentity, out THandler commandHandler);

        if (isTenantHandlerFound)
        {
            return commandHandler;
        }

        var isGeneralHandlerFound = _lifetimeScope.TryResolve(out commandHandler);

        if (isGeneralHandlerFound)
        {
            return commandHandler;
        }

        throw new HandlerNotFoundException(command.GetTypeFullName());
    }
}
