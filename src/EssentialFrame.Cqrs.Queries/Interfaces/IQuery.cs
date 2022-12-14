namespace EssentialFrame.Cqrs.Queries.Interfaces;

public interface IQuery
{
    Guid QueryIdentifier { get; }

    Guid IdentityTenant { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    string ServiceIdentity { get; }
}

public interface IQuery<TResult> : IQuery
{
}
