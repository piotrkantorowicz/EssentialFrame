using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IQueryResult
{
    bool Ok { get; }

    IQueryError ErrorDetails { get; }
}

public interface IQueryResult<out TData> : IQueryResult
{
    TData Data { get; }
}
