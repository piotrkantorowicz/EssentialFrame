using Autofac;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Autofac.Exceptions;
using EssentialFrame.Cqrs.Executors;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Autofac.Executors;

internal sealed class AutofacQueryExecutor : QueryExecutorBase
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacQueryExecutor(ILifetimeScope lifetimeScope) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public override TResult Fetch<TResult>(IQuery<TResult> query)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var isHandlerFound = scope.TryResolve(handlerType, out var queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetTypeFullName());
        }

        var handlerMethod = handlerType
            .GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.Handle));

        if (handlerMethod is null)
        {
            throw new HandlerHandleMethodNotFoundException(query.GetTypeFullName());
        }

        return (TResult)handlerMethod.Invoke(queryHandler,
                                             new object[]
                                             {
                                                 query
                                             });
    }

    public override async Task<TResult> FetchAsync<TResult>(IQuery<TResult> query,
                                                            CancellationToken cancellationToken = default)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var handlerType = typeof(IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var isHandlerFound = scope.TryResolve(handlerType, out var queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetTypeFullName());
        }

        var handlerMethod = handlerType
            .GetMethod(nameof(IAsyncQueryHandler<IQuery<TResult>, TResult>.HandleAsync));

        if (handlerMethod is null)
        {
            throw new HandlerHandleMethodNotFoundException(query.GetTypeFullName());
        }

        return await (handlerMethod.Invoke(queryHandler,
                                           new object[]
                                           {
                                               query,
                                               cancellationToken
                                           }) as Task<TResult>)!;
    }

    protected override THandler FindHandler<TQuery, TResult, THandler>(TQuery query)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        var isHandlerFound = scope.TryResolve(out THandler queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetTypeFullName());
        }

        return queryHandler;
    }
}
