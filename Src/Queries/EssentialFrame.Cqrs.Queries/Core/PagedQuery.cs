using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Queries.Core;

public abstract class PagedQuery<T> : SortOrderQuery<T>, IPagedQuery
{
    protected PagedQuery(SortOrder[] sortOrders, IIdentityContext identityContext) : base(sortOrders, identityContext)
    {
        Page = 1;
        ResultsPerPage = 50;
    }

    protected PagedQuery(int page, int resultsPerPage, SortOrder[] sortOrders, IIdentityContext identityContext) : base(
        sortOrders, identityContext)
    {
        Page = page;
        ResultsPerPage = resultsPerPage;
    }

    protected PagedQuery(Guid queryIdentifier, SortOrder[] sortOrders, IIdentityContext identityContext) : base(
        queryIdentifier, sortOrders, identityContext)
    {
        Page = 1;
        ResultsPerPage = 50;
    }

    protected PagedQuery(Guid queryIdentifier, int page, int resultsPerPage, SortOrder[] sortOrders,
        IIdentityContext identityContext) : base(queryIdentifier, sortOrders, identityContext)
    {
        Page = page;
        ResultsPerPage = resultsPerPage;
    }

    public int Page { get; }

    public int ResultsPerPage { get; }
}