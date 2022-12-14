using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IQueryHandler<in TQuery, out TResult> : IHandler
    where TQuery : class, IQuery<TResult>
{
    TResult Handle(TQuery query);
}
