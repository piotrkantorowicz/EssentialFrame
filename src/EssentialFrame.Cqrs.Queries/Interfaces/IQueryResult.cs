using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Interfaces;

public interface IQueryResult
{
    bool Ok { get; }

    IQueryError ErrorDetails { get; }
}

public interface IQueryResult<out TData> : IQueryResult
{
    TData Data { get; }
}

