using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Core.Interfaces;

public interface IQueryResult
{
    bool Ok { get; }

    IQueryError ErrorDetails { get; }
}

public interface IQueryResult<out TData> : IQueryResult
{
    TData Data { get; }
}

public interface IPagedQueryResult<out TData> : IQueryResult<TData>
{
    public int? Page { get; }

    public int? ResultsPerPage { get; }

    public int? TotalPages { get; }

    public long? TotalResults { get; }
}