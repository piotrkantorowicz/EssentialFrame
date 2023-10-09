namespace EssentialFrame.Cqrs.Queries.Core.Interfaces;

public interface IAsyncQueryHandler<in TQuery, TResult> : IQueryHandler where TQuery : class, IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}

public interface IQueryHandler<in TQuery, out TResult> : IQueryHandler where TQuery : class, IQuery<TResult>
{
    TResult Handle(TQuery query);
}

public interface IQueryHandler
{
}