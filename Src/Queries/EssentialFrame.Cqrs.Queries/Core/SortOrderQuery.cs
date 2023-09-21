using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Core;

public abstract class SortOrderQuery<T> : Query<T>, ISortOrderQuery
{
    protected SortOrderQuery(IEnumerable<SortOrder> sortOrders, IIdentityContext identityContext) : base(
        identityContext)
    {
        SortOrders = sortOrders.ToList();
    }

    protected SortOrderQuery(Guid queryIdentifier, IEnumerable<SortOrder> sortOrders,
        IIdentityContext identityContext) : base(
        queryIdentifier, identityContext)
    {
        SortOrders = sortOrders.ToList();
    }

    public IReadOnlyCollection<SortOrder> SortOrders { get; }
}