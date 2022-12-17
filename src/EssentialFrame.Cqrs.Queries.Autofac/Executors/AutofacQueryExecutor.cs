using Autofac;
using Autofac.Core;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Autofac.Executors;

internal sealed class AutofacQueryExecutor : IQueryExecutor
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacQueryExecutor(ILifetimeScope lifetimeScope) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public TResult Fetch<TResult>(IQuery<TResult> query)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var isHandlerFound = scope.TryResolve(handlerType, out var queryHandler);

        if (!isHandlerFound)
        {
            throw new
                DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                              "Most likely it is not properly registered in container.");
        }

        var handlerMethod = handlerType
            .GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.Handle));

        if (handlerMethod is null)
        {
            throw new
                MissingMethodException($"Handler method for command or query: ({query.GetTypeFullName()}) hasn't been found. " +
                                       "It occurred, because likely handler hasn't has method implementation.");
        }

        return (TResult)handlerMethod.Invoke(queryHandler,
                                             new object[]
                                             {
                                                 query
                                             });
    }

    public TResult Fetch<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult>
        where TResult : class, IQueryResult<TResult>
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        var isHandlerFound = scope.TryResolve<IQueryHandler<TQuery, TResult>>(out var queryHandler);

        if (!isHandlerFound)
        {
            throw new
                DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                              "Most likely it is not properly registered in container.");
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
            throw new
                DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                              "Most likely it is not properly registered in container.");
        }

        var handlerMethod = handlerType
            .GetMethod(nameof(IAsyncQueryHandler<IQuery<TResult>, TResult>.HandleAsync));

        if (handlerMethod is null)
        {
            throw new
                MissingMethodException($"Handler method for command or query: ({query.GetTypeFullName()}) hasn't been found. " +
                                       "It occurred, because likely handler hasn't has method implementation.");
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
        where TResult : class, IQueryResult<TResult>
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var isHandlerFound = scope.TryResolve<IAsyncQueryHandler<TQuery, TResult>>(out var queryHandler);

        if (!isHandlerFound)
        {
            throw new
                DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                              "Most likely it is not properly registered in container.");
        }

        return await queryHandler.HandleAsync(query, cancellationToken);
    }
}

