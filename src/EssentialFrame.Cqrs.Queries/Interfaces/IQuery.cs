namespace EssentialFrame.Cqrs.Queries.Interfaces;

public interface IQuery
{
    Guid QueryIdentifier { get; }

    Guid IdentityTenant { get; }

    Guid IdentityUser { get; }
}

public interface IQuery<TResult> : IQuery
{
}
