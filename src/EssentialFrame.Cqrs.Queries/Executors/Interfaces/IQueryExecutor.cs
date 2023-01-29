using EssentialFrame.Cqrs.Queries.Core.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Executors.Interfaces;

public interface IQueryExecutor
{
    TResult Fetch<TResult>(IQuery<TResult> query);

    TResult Fetch<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
        where TResult : class, IQueryResult<TResult>;

    Task<TResult> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    Task<TResult> FetchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult> where TResult : class, IQueryResult<TResult>;
}