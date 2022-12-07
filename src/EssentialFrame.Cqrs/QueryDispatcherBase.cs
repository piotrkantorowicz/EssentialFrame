using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs;

public class QueryDispatcherBase : IQueryDispatcher
{
    public TResult Query<TResult>(IQuery<TResult> query) => throw new NotImplementedException();

    public TResult Query<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult> where TResult : IQueryResult<TResult> =>
        throw new NotImplementedException();

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult> where TResult : IQueryResult<TResult> =>
        throw new NotImplementedException();
}
