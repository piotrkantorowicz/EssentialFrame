using EssentialFrame.Cqrs.Queries.Const;

namespace EssentialFrame.Cqrs.Queries.Core;

public struct SortOrder
{
    public SortOrder(string orderBy)
    {
        OrderBy = orderBy;
        SortDirection = SortDirections.Ascending;
    }

    public SortOrder(string orderBy, string sortDirection)
    {
        OrderBy = orderBy;

        if (!sortDirection.Equals(SortDirections.Ascending, StringComparison.InvariantCultureIgnoreCase) ||
            !sortDirection.Equals(SortDirections.Descending, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ArgumentException(
                $"Sort direction must be equals to {SortDirections.Ascending} or {SortDirections.Descending} other values are not supported",
                sortDirection);
        }

        SortDirection = sortDirection;
    }

    public string OrderBy { get; }

    public string SortDirection { get; }
}