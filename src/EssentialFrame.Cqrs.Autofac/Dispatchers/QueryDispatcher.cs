using Autofac;
using EssentialFrame.Cqrs.Autofac.Exceptions;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Autofac.Dispatchers;

internal sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly ILifetimeScope _lifetimeScope;

    public QueryDispatcher(ILifetimeScope lifetimeScope) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var isHandlerFound = scope.TryResolve(handlerType, out var queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetType().Name);
        }

        return await (handlerType
                      .GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))
                      ?
                      .Invoke(queryHandler,
                              new object[]
                              {
                                  query,
                                  cancellationToken
                              }) as Task<TResult>)!;
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>
        where TResult : IQueryResult<TResult>
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var isHandlerFound = scope.TryResolve<IQueryHandler<TQuery, TResult>>(out var queryHandler);

        if (!isHandlerFound)
        {
            throw new HandlerNotFoundException(query.GetType().Name);
        }

        return await queryHandler.HandleAsync(query, cancellationToken);
    }
}
