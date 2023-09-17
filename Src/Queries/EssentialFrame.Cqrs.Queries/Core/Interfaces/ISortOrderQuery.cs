namespace EssentialFrame.Cqrs.Queries.Core.Interfaces;

public interface ISortOrderQuery
{
    IReadOnlyCollection<SortOrder> SortOrders { get; }
}