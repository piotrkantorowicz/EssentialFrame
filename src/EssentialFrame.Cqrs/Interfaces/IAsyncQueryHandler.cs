using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IAsyncQueryHandler<in TQuery, TResult> : IHandler
    where TQuery : class, IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
