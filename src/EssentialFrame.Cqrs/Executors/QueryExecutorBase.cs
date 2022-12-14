using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Executors;

public abstract class QueryExecutorBase : IQueryExecutor
{
    public abstract TResult Fetch<TResult>(IQuery<TResult> query);

    public virtual TResult Fetch<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult>
        where TResult : class, IQueryResult<TResult>
    {
        var handler = FindHandler<TQuery, TResult, IQueryHandler<TQuery, TResult>>(query);

        return handler.Handle(query);
    }

    public abstract Task<TResult> FetchAsync<TResult>(IQuery<TResult> query,
                                                      CancellationToken cancellationToken = default);

    public virtual async Task<TResult> FetchAsync<TQuery, TResult>(TQuery query,
                                                                   CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>
        where TResult : class, IQueryResult<TResult>
    {
        var handler = FindHandler<TQuery, TResult, IAsyncQueryHandler<TQuery, TResult>>(query);

        return await handler.HandleAsync(query, cancellationToken);
    }

    protected abstract THandler FindHandler<TQuery, TResult, THandler>(TQuery query)
        where TQuery : class, IQuery<TResult>
        where TResult : class, IQueryResult<TResult>
        where THandler : class, IQueryHandler;
}
