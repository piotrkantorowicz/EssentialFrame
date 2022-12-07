using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    TResult Handle(TQuery query);

    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
