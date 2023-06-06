using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Queries.Core;

public abstract class SortOrderQuery<T> : Query<T>, ISortOrderQuery
{
    protected SortOrderQuery(SortOrder[] sortOrders, IIdentityContext identityContext) : base(identityContext)
    {
        SortOrders = sortOrders;
    }

    protected SortOrderQuery(Guid queryIdentifier, SortOrder[] sortOrders, IIdentityContext identityContext) : base(
        queryIdentifier, identityContext)
    {
        SortOrders = sortOrders;
    }

    public SortOrder[] SortOrders { get; }
}