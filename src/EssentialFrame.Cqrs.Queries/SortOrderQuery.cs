using EssentialFrame.Cqrs.Queries.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Queries;

public abstract class SortOrderQuery<T> : Query<T>, ISortOrderQuery
{
    protected SortOrderQuery(SortOrder[] sortOrders, IIdentity identity)
        : base(identity) =>
        SortOrders = sortOrders;

    protected SortOrderQuery(Guid queryIdentifier,
                             SortOrder[] sortOrders,
                             IIdentity identity)
        : base(queryIdentifier, identity) =>
        SortOrders = sortOrders;

    public SortOrder[] SortOrders { get; }
}
