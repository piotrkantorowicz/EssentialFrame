namespace EssentialFrame.Cqrs.Queries.Core.Interfaces;

public interface IPagedQuery
{
    int Page { get; }

    int ResultsPerPage { get; }

    IReadOnlyCollection<SortOrder> SortOrders { get; } 
}