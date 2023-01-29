using System.Reflection;
using Autofac;
using Autofac.Core;
using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Cqrs.Queries.Executors.Interfaces;
using EssentialFrame.Extensions;

namespace EssentialFrame.Cqrs.Queries.Executors;

internal sealed class DefaultQueryExecutor : IQueryExecutor
{
    private readonly ILifetimeScope _lifetimeScope;

    public DefaultQueryExecutor(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
    }

    public TResult Fetch<TResult>(IQuery<TResult> query)
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        bool isHandlerFound = scope.TryResolve(handlerType, out object queryHandler);

        if (!isHandlerFound)
        {
            throw new DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                                    "Most likely it is not properly registered in container.");
        }

        MethodInfo handlerMethod = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.Handle));

        if (handlerMethod is null)
        {
            throw new MissingMethodException(
                $"Handler method for command or query: ({query.GetTypeFullName()}) hasn't been found. " +
                "It occurred, because likely handler hasn't has method implementation.");
        }

        return (TResult)handlerMethod.Invoke(queryHandler, new object[] { query });
    }

    public TResult Fetch<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
        where TResult : class, IQueryResult<TResult>
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        bool isHandlerFound = scope.TryResolve(out IQueryHandler<TQuery, TResult> queryHandler);

        if (!isHandlerFound)
        {
            throw new DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                                    "Most likely it is not properly registered in container.");
        }

        return queryHandler.Handle(query);
    }

    public async Task<TResult> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        Type handlerType = typeof(IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        bool isHandlerFound = scope.TryResolve(handlerType, out object queryHandler);

        if (!isHandlerFound)
        {
            throw new DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                                    "Most likely it is not properly registered in container.");
        }

        MethodInfo handlerMethod =
            handlerType.GetMethod(nameof(IAsyncQueryHandler<IQuery<TResult>, TResult>.HandleAsync));

        if (handlerMethod is null)
        {
            throw new MissingMethodException(
                $"Handler method for command or query: ({query.GetTypeFullName()}) hasn't been found. " +
                "It occurred, because likely handler hasn't has method implementation.");
        }

        return await (handlerMethod.Invoke(queryHandler, new object[] { query, cancellationToken }) as Task<TResult>)!;
    }

    public async Task<TResult> FetchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult> where TResult : class, IQueryResult<TResult>
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        bool isHandlerFound = scope.TryResolve(out IAsyncQueryHandler<TQuery, TResult> queryHandler);

        if (!isHandlerFound)
        {
            throw new DependencyResolutionException($"Unable to resolve handler for {query.GetTypeFullName()}. " +
                                                    "Most likely it is not properly registered in container.");
        }

        return await queryHandler.HandleAsync(query, cancellationToken);
    }
}