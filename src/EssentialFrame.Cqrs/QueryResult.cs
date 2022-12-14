using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs;

public sealed class QueryResult<T> : IQueryResult<T>
{
    private QueryResult()
    {
    }

    public bool Ok { get; private init; }

    public T Data { get; private init; }

    public IQueryError ErrorDetails { get; private init; }

    public static QueryResult<T> Success(T data) =>
        new()
        {
            Ok = true,
            Data = data
        };

    public static QueryResult<T> Fail(IQueryError queryError) =>
        new()
        {
            Ok = false,
            ErrorDetails = queryError
        };
}
