using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IQueryDispatcher
{
    TResult Fetch<TResult>(IQuery<TResult> query);

    TResult Fetch<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult>
        where TResult : IQueryResult<TResult>;

    Task<TResult> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    Task<TResult> FetchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>
        where TResult : IQueryResult<TResult>;
}
