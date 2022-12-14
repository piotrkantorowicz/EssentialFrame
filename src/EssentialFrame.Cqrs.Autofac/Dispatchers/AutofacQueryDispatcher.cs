using Autofac;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Autofac.Exceptions;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Autofac.Dispatchers;

internal sealed class AutofacQueryDispatcher : IQueryDispatcher
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacQueryDispatcher(ILifetimeScope lifetimeScope) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public TResult Fetch<TResult>(IQuery<TResult> query)
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

    public TResult Fetch<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult> where TResult : IQueryResult<TResult>
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        var isHandlerFound = scope.TryResolve<IQueryHandler<TQuery, TResult>>(out var queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetTypeFullName());
        }

        return queryHandler.Handle(query);
    }

    public async Task<TResult> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
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

    public async Task<TResult> FetchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>
        where TResult : IQueryResult<TResult>
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var isHandlerFound = scope.TryResolve<IAsyncQueryHandler<TQuery, TResult>>(out var queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetTypeFullName());
        }

        return await queryHandler.HandleAsync(query, cancellationToken);
    }
}
