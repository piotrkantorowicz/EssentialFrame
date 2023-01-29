namespace EssentialFrame.Cqrs.Queries.Core.Interfaces;

public interface IPagedQuery
{
    int Page { get; }

    int ResultsPerPage { get; }

    SortOrder[] SortOrders { get; }
}