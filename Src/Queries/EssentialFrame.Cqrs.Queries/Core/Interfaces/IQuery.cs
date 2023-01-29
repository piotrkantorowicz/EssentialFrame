namespace EssentialFrame.Cqrs.Queries.Core.Interfaces;

public interface IQuery
{
    Guid QueryIdentifier { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    string ServiceIdentity { get; }
}

public interface IQuery<TResult> : IQuery
{
}