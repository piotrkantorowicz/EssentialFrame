namespace EssentialFrame.Cqrs.Queries.Interfaces;

public interface IPagedQuery
{
    int Page { get; }

    int ResultsPerPage { get; }

    SortOrder[] SortOrders { get; }
}



