using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Core;

public sealed class QueryResult<T> : IQueryResult<T>
{
    private QueryResult()
    {
    }

    public bool Ok { get; private init; }

    public T Data { get; private init; }

    public IQueryError ErrorDetails { get; private init; }

    public static QueryResult<T> Success(T data)
    {
        return new QueryResult<T> { Ok = true, Data = data };
    }

    public static QueryResult<T> Fail(IQueryError queryError)
    {
        return new QueryResult<T> { Ok = false, ErrorDetails = queryError };
    }
}