using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IQueryDispatcher
{
    TResult Query<TResult>(IQuery<TResult> query);

    TResult Query<TQuery, TResult>(TQuery query)
        where TQuery : class, IQuery<TResult>
        where TResult : IQueryResult<TResult>;

    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TResult>
        where TResult : IQueryResult<TResult>;
}
