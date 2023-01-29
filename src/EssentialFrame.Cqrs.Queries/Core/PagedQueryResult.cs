using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Core;

public sealed class PagedQueryResult<T> : IPagedQueryResult<T>
{
    private PagedQueryResult()
    {
    }

    public bool Ok { get; private init; }

    public T Data { get; private init; }

    public int? Page { get; private init; }

    public int? ResultsPerPage { get; private init; }

    public int? TotalPages { get; private init; }

    public long? TotalResults { get; private init; }

    public IQueryError ErrorDetails { get; private init; }

    public static PagedQueryResult<T> Success(T data, int page, int resultsPerPage, int totalPages, int totalResults)
    {
        return new PagedQueryResult<T>
        {
            Ok = true,
            Page = page,
            ResultsPerPage = resultsPerPage,
            TotalPages = totalPages,
            TotalResults = totalResults,
            Data = data
        };
    }

    public static PagedQueryResult<T> Fail(IQueryError queryError)
    {
        return new PagedQueryResult<T> { Ok = false, ErrorDetails = queryError };
    }
}