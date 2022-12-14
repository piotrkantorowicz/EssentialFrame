using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Dispatchers;

public abstract class QueryDispatcherBase : IQueryDispatcher
{
    public TResult Fetch<TResult>(IQuery<TResult> query) => throw new NotImplementedException();

    public TResult Fetch<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult> where TResult : IQueryResult<TResult> =>
        throw new NotImplementedException();

    public Task<TResult> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<TResult> FetchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult> where TResult : IQueryResult<TResult> =>
        throw new NotImplementedException();
}
